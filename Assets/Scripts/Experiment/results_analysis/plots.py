import numpy as np
import matplotlib.pyplot as plt
from collections import Counter

# -----------------------
# Helpers
# -----------------------
def calculate_percentages(scores, max_score):
    """Calculate percentage scores."""
    return [score / max_score * 100 for score in scores]

def mean_and_std(arr):
    """Return (mean, sample‐std)."""
    return arr.mean(), arr.std(ddof=1)

def clip_errors(means, errs):
    """Clip symmetric error bars so they stay within [-2, 2]."""
    lower = [min(e, m + 2) for m, e in zip(means, errs)]
    upper = [min(e, 2 - m) for m, e in zip(means, errs)]
    return lower, upper

def parse_gestures(data_list):
    """Flatten list of comma‐separated gesture strings."""
    all_g = []
    for entry in data_list:
        all_g.extend(g.strip() for g in entry.split(','))
    return all_g

# -----------------------
# Plotting functions
# -----------------------
def plot_boxplot(data, labels, title, xlabel="Percentage Score", xlim=(0,100)):
    plt.figure(figsize=(8, 6))
    plt.boxplot(data, labels=labels, vert=False)
    plt.xlabel(xlabel)
    plt.title(title)
    plt.xlim(*xlim)
    plt.grid(True)
    plt.tight_layout()
    plt.show()

def plot_easy_medium_modes():
    # ...existing code for easy and medium scores, using calculate_percentages & plot_boxplot...
    easy_ht_scores = [34,34,33,31,33,35,28,32,27]
    easy_cont_scores = [37,37,36,37,37,36,37,35,37]
    easy_max = 37
    e_ht_pct = calculate_percentages(easy_ht_scores, easy_max)
    e_ct_pct = calculate_percentages(easy_cont_scores, easy_max)
    plot_boxplot([e_ht_pct, e_ct_pct],
                 labels=['Hand\nTracking','Controller'],
                 title='Percentage Score for Easy Mode')

    # medium
    medium_ht_scores = [33,46,39,41,45,46,34,41,39]
    medium_cont_scores = [42,54,51,50,49,52,53,43,48]
    medium_max = 56
    m_ht_pct = calculate_percentages(medium_ht_scores, medium_max)
    m_ct_pct = calculate_percentages(medium_cont_scores, medium_max)
    plot_boxplot([m_ht_pct, m_ct_pct],
                 labels=['Hand\nTracking','Controller'],
                 title='Percentage Score for Medium Mode')

def plot_engagement_and_ease():
    engagement = np.array([5,5,5,4,2,4,4,3,2]) - 3
    ease      = np.array([4,2,2,1,1,1,2,1,1]) - 3
    plot_boxplot([engagement], ['Engagement'], 
                 title='Normalized Mean Likert Scores for Engagement',
                 xlabel='Normalized Score', xlim=(-2,2))
    plot_boxplot([ease], ['Ease of Use'], 
                 title='Normalized Mean Likert Scores for Ease of Use',
                 xlabel='Normalized Score', xlim=(-2,2))

def plot_two_condition_metrics(show_error_bars=True):
    fn_ht = np.array([0,-1,1,1,1,-1,1,2,-1])
    fn_ct = np.array([-2,-2,-2,-2,-2,-2,-2,-2,-2])
    pr_ht = np.array([2,2,2,1,0,2,2,1,2])
    pr_ct = np.array([-1,2,0,0,2,-1,0,0,2])
    labels = ['Hand Tracking','Controllers']

    means_fn = [*mean_and_std(fn_ht)[:1], *mean_and_std(fn_ct)[:1]]
    stds_fn  = [*mean_and_std(fn_ht)[1:], *mean_and_std(fn_ct)[1:]]
    means_pr = [*mean_and_std(pr_ht)[:1], *mean_and_std(pr_ct)[:1]]
    stds_pr  = [*mean_and_std(pr_ht)[1:], *mean_and_std(pr_ct)[1:]]

    if show_error_bars:
        lf, uf = clip_errors(means_fn, stds_fn)
        yerr_fn = [lf, uf]
        lp, up = clip_errors(means_pr, stds_pr)
        yerr_pr = [lp, up]
    else:
        yerr_fn = yerr_pr = None

    # chart 1
    fig, ax = plt.subplots(figsize=(6,4))
    x = np.arange(len(labels))
    ax.bar(x, means_fn, yerr=yerr_fn, capsize=5 if show_error_bars else 0, color='#1f77b4')
    ax.set(xticks=x, xticklabels=labels, ylabel='Mean Likert (-2,2)',
           title='Unrecognized Inputs (False Negatives)')
    ax.axhline(0, color='k'); ax.set_ylim(-2.2,2.2)
    plt.tight_layout(); plt.show()

    # chart 2
    fig, ax = plt.subplots(figsize=(6,4))
    ax.bar(x, means_pr, yerr=yerr_pr, capsize=5 if show_error_bars else 0, color='#ff7f0e')
    ax.set(xticks=x, xticklabels=labels, ylabel='Mean Likert (-2,2)', title='Presence')
    ax.axhline(0, color='k'); ax.set_ylim(-2.2,2.2)
    plt.tight_layout(); plt.show()

def plot_single_condition_metrics(show_error_bars=True):
    fp_ht = np.array([-2,-2,-1,0,-2,-1,-2,1,1])
    nat_ht = np.array([1,2,1,1,2,0,2,2,2])
    learn = np.array([2,2,1,-1,1,2,2,1,-2])

    labels = ['Phantom Inputs','Naturalness','Learnability']
    means = [*mean_and_std(fp_ht)[:1], *mean_and_std(nat_ht)[:1], *mean_and_std(learn)[:1]]
    stds  = [*mean_and_std(fp_ht)[1:], *mean_and_std(nat_ht)[1:], *mean_and_std(learn)[1:]]

    if show_error_bars:
        lo, up = clip_errors(means, stds)
        yerr = [lo, up]
    else:
        yerr = None

    fig, ax = plt.subplots(figsize=(8,6))
    x = np.arange(len(labels))
    ax.bar(x, means, yerr=yerr, capsize=5 if show_error_bars else 0, color='#2ca02c')
    ax.set(xticks=x, xticklabels=labels, ylabel='Mean Likert (-2,2)',
           title='Analysis of Phantom Inputs, Naturalness, and Learnability')
    ax.axhline(0, color='k'); ax.set_ylim(-2.2,2.2)
    plt.tight_layout(); plt.show()

def plot_pose_gesture_comparison():
    easiest_data = [
        "Fist pose, Thumbs Up pose, Gun pose",
        "Punch, Thumbs Up pose, Gun pose",
        "Right swipe, Punch, Gun pose",
        "Up swipe, Punch, Gun pose",
        "Punch, Thumbs Up pose, Gun pose",
        "Down swipe, Fist pose, Gun pose",
        "Fist pose, Thumbs Up pose, Gun pose",
        "Punch, Thumbs Up pose, Gun pose",
    ]

    engaging_data = [
        "Punch, Thumbs Up pose, Gun pose",
        "Fist pose, Thumbs Up pose, Gun pose",
        "Right swipe, Punch, Gun pose",
        "Left swipe, Punch, Gun pose",
        "Punch, Thumbs Up pose, Gun pose",
        "Fist pose, Thumbs Up pose, Gun pose",
        "Punch, Thumbs Up pose, Gun pose",
        "Punch, Thumbs Up pose, Gun pose",
    ]
    parsed_e = parse_gestures(easiest_data)
    parsed_en= parse_gestures(engaging_data)
    cnt_e  = Counter(parsed_e)
    cnt_en = Counter(parsed_en)
    all_gs = sorted(set(cnt_e)|set(cnt_en))
    freq_e  = [cnt_e[g] for g in all_gs]
    freq_en = [cnt_en[g] for g in all_gs]

    x = np.arange(len(all_gs))
    w = 0.4
    fig, ax = plt.subplots(figsize=(10,6))
    ax.bar(x - w/2, freq_e, width=w, label='Easiest', color='#1f77b4')
    ax.bar(x + w/2, freq_en, width=w, label='Most Engaging', color='#ff7f0e')
    ax.set(xticks=x, xticklabels=all_gs, xlabel='Gestures/Poses',
           ylabel='Frequency', title='Easiest vs Engaging Gestures')
    ax.legend(); plt.xticks(rotation=45, ha='right')
    plt.tight_layout(); plt.show()


def main():
    plot_easy_medium_modes()
    plot_engagement_and_ease()
    plot_two_condition_metrics(show_error_bars=True)
    plot_single_condition_metrics(show_error_bars=True)
    plot_pose_gesture_comparison()

if __name__ == "__main__":
    main()