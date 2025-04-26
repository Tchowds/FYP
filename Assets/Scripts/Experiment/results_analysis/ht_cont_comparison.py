from scipy import stats
import numpy as np

false_negatives_hand_tracking = np.array([0, -1, 1, 1, 1, -1, 1, 2, -1])
false_negatives_controllers   = np.array([-2, -2, -2, -2, -2, -2, -2, -2, -2])
presence_hand_tracking        = np.array([2, 2, 2, 1, 0, 2, 2, 1, 2])
presence_controllers          = np.array([-1, 2, 0, 0, 2, -1, 0, 0, 2])

def paired_t_test(a, b, label_a, label_b, alpha=0.05):
    """
    Perform a paired t‑test between two arrays.
    Returns t-statistic and p-value, and comments on significance.
    """
    t_stat, p_val = stats.ttest_rel(a, b)
    signif = "significant" if p_val < alpha else "not significant"
    print(
        f"Paired t-test {label_a} vs {label_b} → "
        f"t = {t_stat:.3f}, p = {p_val:.3f} "
        f"→ {signif} (α = {alpha})"
    )
    return t_stat, p_val

def compute_likert_stats(data, label, ci=0.95):
    """Compute mean and half-width of the CI error bar for Likert data."""
    mean = np.mean(data)
    sem = stats.sem(data)
    n = len(data)
    # t multiplier for two‐sided interval
    t_val = stats.t.ppf((1 + ci) / 2, df=n-1)
    h = sem * t_val
    print(f"{label} → Mean: {mean:.2f}, ±{h:.2f} ({int(ci*100)}% CI)")
    return mean, h

if __name__ == "__main__":
    compute_likert_stats(false_negatives_hand_tracking, "False Neg. Hand Tracking")
    compute_likert_stats(false_negatives_controllers,   "False Neg. Controllers")
    compute_likert_stats(presence_hand_tracking,        "Presence Hand Tracking")
    compute_likert_stats(presence_controllers,          "Presence Controllers")

    # Paired t-tests
    paired_t_test(false_negatives_hand_tracking, false_negatives_controllers,
                  "False Neg. Hand Tracking", "False Neg. Controllers")
    paired_t_test(presence_hand_tracking, presence_controllers,
                  "Presence Hand Tracking", "Presence Controllers")
    