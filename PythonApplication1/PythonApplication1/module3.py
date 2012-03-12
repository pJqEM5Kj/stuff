import math


def problem():
    N = 1211
    print('Number = ' + str(N))
    res = parse(N)
    print(res)
    L_R = check(res)
    print(L_R)

def check(list):
    L, R = 0, 1
    for i in list:
        if i == 'L':
            L, R = 2*L - R, R
        else:
            L, R = L, 2*R - L
    return L, R

def parse(N):
    if N == 0 or N == 1:
        return []

    power = math.log(abs(N), 2)
    power = int(power) + 1

    L, R = 0, 0
    if N > 0:
        L, R = N - 2**power, N
    else:
        L, R = N, 2**power + N

    res = []
    while power > 0:
        power -= 1
        if abs(L) < abs(R):
            L, R = L, 2**power + L
            res.append('R')
        else:
            L, R = R - 2**power, R
            res.append('L')

    res = list(reversed(res))
    return res;


def main():
    print('exec main from module: {0}'.format(__name__))
    print()

    #------------------------------------------------------------

    problem()

    #
    pass
