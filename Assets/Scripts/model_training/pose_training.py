import json
import torch
import torch.nn as nn
import torch.optim as optim
from torch.utils.data import DataLoader, TensorDataset
from pull_push_data import sync_data
import os

device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
print(f"Using device: {device}")

def load_pose_data(json_path: str, batch_size: int) -> (DataLoader, int, int):
    """
    Loads gesture data from a JSON file, preprocesses it into inputs and labels,
    wraps it in a DataLoader, and returns that loader along with input feature size
    and number of classes.
    """
    with open(json_path, 'r') as f:
        js = json.load(f)

    data = []
    for pose in js["poses"].values():
        label = pose["poseGestureIndex"]
        for feat in pose["poseGestureData"]:
            data.append(feat + [label])

    inputs = torch.tensor([d[:-1] for d in data], dtype=torch.float32)
    labels = torch.tensor([d[-1] for d in data], dtype=torch.int64)
    
    dataset = TensorDataset(inputs, labels)
    input_size = inputs.size(1)
    num_classes = labels.unique().numel()
    loader = DataLoader(dataset, batch_size=batch_size, shuffle=True)
    
    print(f"Loaded poses: input_size={input_size}, classes={num_classes}")
    return loader, input_size, num_classes

# =============================================================================
# Model Architecture
# =============================================================================
class Net(nn.Module):
    def __init__(self, input_size: int, num_classes: int):
        super(Net, self).__init__()
        self.fc1 = nn.Linear(input_size, 64)
        self.fc2 = nn.Linear(64, 32)
        self.fc3 = nn.Linear(32, num_classes)
        self.relu = nn.ReLU()

    def forward(self, x):
        x = self.relu(self.fc1(x))
        x = self.relu(self.fc2(x))
        return self.fc3(x)

# =============================================================================
# Training and Export Functions
# =============================================================================
def train_model(model, dataloader, criterion, optimizer, device, epochs=100):
    model.to(device)
    for ep in range(epochs):
        model.train()
        running_loss, correct, total = 0.0, 0, 0
        for xb, yb in dataloader:
            xb, yb = xb.to(device), yb.to(device)
            optimizer.zero_grad()
            out = model(xb)
            loss = criterion(out, yb)
            loss.backward()
            optimizer.step()
            running_loss += loss.item()
            preds = out.argmax(dim=1)
            total += yb.size(0)
            correct += (preds == yb).sum().item()
        if ep == 0 or (ep + 1) % 10 == 0:
            acc = 100 * correct / total
            print(f"Epoch[{ep+1}/{epochs}] Loss:{running_loss/len(dataloader):.4f} Acc:{acc:.2f}%")
    print("Training complete")
    return model

def export_to_onnx(model, input_size, device, path):
    model.eval()
    dummy = torch.randn(1, input_size, device=device)
    torch.onnx.export(
        model, dummy, path,
        input_names=["input"], output_names=["output"], verbose=False
    )
    print(f"Exported ONNX to {path}")

# =============================================================================
# Main Function
# =============================================================================
def main():
    # Configurations
    json_file = "poseGestureData.json"
    batch_size = 32
    epochs = 100
    link = True  # Set to False for adb
    
    # Sync data from HMD
    sync_data(pull=True, push=False, link=link)

    # Load data
    dataloader, input_size, num_classes = load_pose_data(json_file, batch_size)

    net = Net(input_size, num_classes)
    crit = nn.CrossEntropyLoss()
    opt = optim.SGD(net.parameters(), lr=0.001)

    trained = train_model(net, dataloader, crit, opt, device, epochs)
    os.makedirs("models", exist_ok=True)
    export_to_onnx(trained, input_size, device, "models/model_poses.onnx")

if __name__ == "__main__":
    main()