import numpy as np
from scipy import stats
from plots import calculate_percentages

def significance_between_modes(scores1, max1, scores2, max2, alpha=0.05):
    """
    Perform an independent-samples t-test (Welch's) on two sets of percentage scores.
    Returns t-statistic, p-value, and a boolean indicating if p < alpha.
    """
    pct1 = calculate_percentages(scores1, max1)
    pct2 = calculate_percentages(scores2, max2)
    t_stat, p_value = stats.ttest_ind(pct1, pct2, equal_var=False)
    significant = p_value < alpha
    return t_stat, p_value, significant

if __name__ == "__main__":
    # Easy mode data
    easy_ht_scores = [34, 34, 33, 31, 33, 35, 28, 32, 27]
    # mean = 31.88  -> 86.16
    easy_cont_scores = [37, 37, 36, 37, 37, 36, 37, 35, 37]
    # mean = 36.556 -> 98.8
    easy_max = 37

    # Medium mode data
    medium_ht_scores = [33, 46, 39, 41, 45, 46, 34, 41, 39]
    # mean = 40.44 -> 72.22
    medium_cont_scores = [42, 54, 51, 50, 49, 52, 53, 43, 48]
    # mean = 49.11 -> 87.70
    medium_max = 56

    # Run tests
    easy_t, easy_p, easy_sig = significance_between_modes(
        easy_ht_scores, easy_max, easy_cont_scores, easy_max
    )
    medium_t, medium_p, medium_sig = significance_between_modes(
        medium_ht_scores, medium_max, medium_cont_scores, medium_max
    )

    # Print results
    print(f"Easy modes comparison: t = {easy_t:.3f}, p = {easy_p:.3f}, significant? {easy_sig}")
    print(f"Medium modes comparison: t = {medium_t:.3f}, p = {medium_p:.3f}, significant? {medium_sig}")
