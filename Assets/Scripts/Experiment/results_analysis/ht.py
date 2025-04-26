import numpy as np
from ht_cont_comparison import compute_likert_stats

false_positives_hand_tracking = np.array([-2, -2, -1, 0, -2, -1, -2, 1, 1])
naturalness_hand_tracking     = np.array([1, 2, 1, 1, 2, 0, 2, 2, 2])
learnability                  = np.array([2, 2, 1, -1, 1, 2, 2, 1, -2])


if __name__ == '__main__':
    for name, arr in [
        ("False Positives", false_positives_hand_tracking),
        ("Naturalness",     naturalness_hand_tracking),
        ("Learnability",    learnability)
    ]:
        m, e = compute_likert_stats(arr, name)
        # print(f"{name} → Mean: {m:.2f}, ±{e:.2f} ({int(0.95*100)}% CI)")