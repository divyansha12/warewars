import random
import numpy as np

SIZE = 5
PACKAGE = (0, 4)
DROP = (4, 0)
MOVES = [(0, 1), (0, -1), (-1, 0), (1, 0)]  # up, down, left, right

def step(state, a):
    x, y, has = state
    dx, dy = MOVES[a]
    nx = min(max(x + dx, 0), SIZE - 1)
    ny = min(max(y + dy, 0), SIZE - 1)
    reward, done = -0.1, False
    if (nx, ny) == PACKAGE and not has:
        has, reward = 1, 1.0
    if (nx, ny) == DROP and has:
        reward, done = 10.0, True
    return (nx, ny, has), reward, done

Q = np.zeros((SIZE, SIZE, 2, 4))
alpha, gamma = 0.1, 0.95
episode_rewards = []

for ep in range(3000):
    eps = max(0.05, 1.0 - ep / 1500)
    state, total = (0, 0, 0), 0
    for _ in range(100):
        if random.random() < eps:
            a = random.randrange(4)
        else:
            a = int(np.argmax(Q[state]))
        nxt, r, done = step(state, a)
        Q[state][a] += alpha * (r + gamma * np.max(Q[nxt]) * (not done) - Q[state][a])
        state, total = nxt, total + r
        if done:
            break
    episode_rewards.append(total)

print("Average reward, first 100 episodes:", round(np.mean(episode_rewards[:100]), 2))
print("Average reward, last 100 episodes:", round(np.mean(episode_rewards[-100:]), 2))

try:
    import matplotlib
    matplotlib.use("Agg")
    import matplotlib.pyplot as plt
    plt.plot(np.convolve(episode_rewards, np.ones(50) / 50, mode="valid"))
    plt.xlabel("Episode")
    plt.ylabel("Reward (smoothed)")
    plt.title("Q-learning: tabular warehouse")
    plt.savefig("docs/qlearning_curve.png")
    print("Saved docs/qlearning_curve.png")
except ImportError:
    print("matplotlib not installed, skipping the plot")
