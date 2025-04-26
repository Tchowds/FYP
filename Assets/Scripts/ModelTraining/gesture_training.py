import json
import torch
import torch.nn as nn
import torch.optim as optim
from torch.utils.data import DataLoader, TensorDataset
from pull_push_data import sync_data
import os

# Set device
device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')
print(f"Using device: {device}")

# =============================================================================
# Data Loading and Preprocessing
# =============================================================================
def load_data(json_path: str, batch_size: int, shuffle: bool = True) -> (DataLoader, int, int):
    """
    Loads gesture data from a JSON file, preprocesses it into inputs and labels,
    wraps it in a DataLoader, and returns that loader along with input feature size
    and number of classes.
    """
    with open(json_path, 'r') as f:
        json_data = json.load(f)

    # Extract features and labels from JSON
    data = []
    for gesture_name, gesture_data in json_data["gestures"].items():
        label = gesture_data["poseGestureIndex"]
        for gesture in gesture_data["poseGestureData"]:
            data.append(gesture + [label])

    # Separate inputs and labels
    inputs = [row[:-1] for row in data]
    labels = [row[-1] for row in data]

    # Convert to tensors
    inputs_tensor = torch.tensor(inputs, dtype=torch.float32)
    labels_tensor = torch.tensor(labels, dtype=torch.int64)

    # Build dataset and loader
    dataset = TensorDataset(inputs_tensor, labels_tensor)
    dataloader = DataLoader(dataset, batch_size=batch_size, shuffle=shuffle)

    input_size = inputs_tensor.shape[1]
    num_classes = labels_tensor.unique().numel()
    print(f"Input size: {input_size}, Number of classes: {num_classes}")

    return dataloader, input_size, num_classes

# =============================================================================
# Model Architectures
# =============================================================================
class LowNet(nn.Module):
    """
    A low complexity model: a single hidden layer network.
    """
    def __init__(self, input_size: int, num_classes: int):
        super(LowNet, self).__init__()
        self.fc1 = nn.Linear(input_size, 64)
        self.act = nn.ReLU()
        self.fc2 = nn.Linear(64, num_classes)
    
    def forward(self, x):
        x = self.act(self.fc1(x))
        return self.fc2(x)


class MediumNet(nn.Module):
    """
    A medium complexity model: two hidden layers with batch normalization and dropout.
    """
    def __init__(self, input_size: int, num_classes: int):
        super(MediumNet, self).__init__()
        self.fc1 = nn.Linear(input_size, 256)
        self.bn1 = nn.BatchNorm1d(256)
        self.act1 = nn.ReLU()
        self.dropout1 = nn.Dropout(0.5)
        
        self.fc2 = nn.Linear(256, 128)
        self.bn2 = nn.BatchNorm1d(128)
        self.act2 = nn.ReLU()
        self.dropout2 = nn.Dropout(0.5)
        
        self.fc3 = nn.Linear(128, num_classes)
    
    def forward(self, x):
        x = self.dropout1(self.act1(self.bn1(self.fc1(x))))
        x = self.dropout2(self.act2(self.bn2(self.fc2(x))))
        return self.fc3(x)


class HighNet(nn.Module):
    """
    A high complexity model: three hidden layers with increased number of neurons,
    batch normalization, and dropout.
    """
    def __init__(self, input_size: int, num_classes: int):
        super(HighNet, self).__init__()
        self.fc1 = nn.Linear(input_size, 512)
        self.bn1 = nn.BatchNorm1d(512)
        self.act1 = nn.ReLU()
        self.dropout1 = nn.Dropout(0.5)
        
        self.fc2 = nn.Linear(512, 256)
        self.bn2 = nn.BatchNorm1d(256)
        self.act2 = nn.ReLU()
        self.dropout2 = nn.Dropout(0.5)
        
        self.fc3 = nn.Linear(256, 128)
        self.bn3 = nn.BatchNorm1d(128)
        self.act3 = nn.ReLU()
        self.dropout3 = nn.Dropout(0.5)
        
        self.fc4 = nn.Linear(128, num_classes)
    
    def forward(self, x):
        x = self.dropout1(self.act1(self.bn1(self.fc1(x))))
        x = self.dropout2(self.act2(self.bn2(self.fc2(x))))
        x = self.dropout3(self.act3(self.bn3(self.fc3(x))))
        return self.fc4(x)

# =============================================================================
# Training and Export Functions
# =============================================================================
def train_model(model: nn.Module, dataloader: DataLoader, criterion, optimizer, device, num_epochs: int = 200):
    """
    Trains the given model using the provided DataLoader, loss criterion, and optimizer.
    """
    model.to(device)
    for epoch in range(num_epochs):
        model.train()
        running_loss = 0.0
        correct = 0
        total = 0
        for inputs_batch, labels_batch in dataloader:
            inputs_batch, labels_batch = inputs_batch.to(device), labels_batch.to(device)
            
            optimizer.zero_grad()
            outputs = model(inputs_batch)
            loss = criterion(outputs, labels_batch)
            loss.backward()
            optimizer.step()
            
            running_loss += loss.item()
            _, predicted = torch.max(outputs, 1)
            total += labels_batch.size(0)
            correct += (predicted == labels_batch).sum().item()
        if (epoch + 1) % 10 == 0 or epoch == 0:
            accuracy = 100 * correct / total
            print(f'Epoch [{epoch + 1}/{num_epochs}] Loss: {running_loss / len(dataloader):.4f} Accuracy: {accuracy:.2f}%')
    print('Finished Training')
    return model

def export_to_onnx(model: nn.Module, input_size: int, device, export_path: str):
    """
    Exports the given model to ONNX format.
    """
    model.eval()
    dummy_input = torch.randn(1, input_size, device=device)
    torch.onnx.export(
        model,
        dummy_input,
        export_path,
        input_names=["input"],
        output_names=["output"],
        verbose=False
    )
    print(f"Model exported to {export_path}")

# =============================================================================
# Main Function
# =============================================================================
def main():
    # Configurations
    json_file = 'poseGestureData.json'
    batch_size = 32
    num_epochs = 200
    base_model_name = "model_gestures"
    link = True  # Set to False if using adb

    # Sync data from HMD
    sync_data(pull=True, push=False, link=link)
    
    # Load data
    dataloader, input_size, num_classes = load_data(json_file)
    
    # Define models with different complexities
    models = {
        "low": LowNet(input_size, num_classes),
        "medium": MediumNet(input_size, num_classes),
        "high": HighNet(input_size, num_classes)
    }
    
    criterion = nn.CrossEntropyLoss()
    
    # Train and export each model
    for complexity, model in models.items():
        print(f"\nTraining {complexity} complexity model:")
        optimizer = optim.SGD(model.parameters(), lr=0.001)
        trained_model = train_model(model, dataloader, criterion, optimizer, device, num_epochs)
        models_dir = "models"
        os.makedirs(models_dir, exist_ok=True)
        onnx_path = os.path.join(models_dir, f"{base_model_name}_{complexity}.onnx")
        export_to_onnx(trained_model, input_size, device, onnx_path)

if __name__ == '__main__':
    main()
