import math
import timeit


def main():
    print('exec main from module: {0}'.format(__name__))
    print()

    #------------------------------------------------------------

    #fastPowTest()

    #splitTest()

    #timeTestLauncher()

    rangeSumTest()

    #
    pass


def rangeSumTest():
    num = 3201
    digitCount = 10

    x2 = funcOptimized(num, digitCount)
    print('Last {0} digits optimized = {1}'.format(digitCount, x2))

    x = func(num)
    lastDigits = getLastDigits(x, digitCount)
    print('Last {0} digits = {1}'.format(digitCount, lastDigits))

    print('Equal: {0}'.format(lastDigits == x2))
    pass


def timeTestLauncher():
    t = timeit.Timer("timeTestFunc()", "from module1 import timeTestFunc")

    repNum = 5
    tt = t.timeit(number = repNum) / repNum
    #tt = t.repeat(repeat = 3, number = repNum)
    print(tt)


def timeTestFunc():
    #1------------------------------------------
    for i in range(10000):
        getNotZeroBitPositions(i)
    #-------------------------------------------

    #2------------------------------------------
    num = 3201
    digitCount = 10
    x = func(num)
    #lastDigits = getLastDigits(x, digitCount)
    #
    #x2 = funcOptimized(num, digitCount)
    #-------------------------------------------

    #
    pass


def splitTest():
    num = 5
    print('Num = {0}, bit length = {1}'.format(num, num.bit_length()))

    p1 = getNotZeroBitPositions(num)
    print(p1)

    p2 = getNotZeroBitPositions2(num)
    print(p2)

    p3 = getNotZeroBitPositions3(num)
    print(p3)

    pass


def func(x):
    #1
    #sum = 0
    #for i in range(1, x):
    #    sum += i ** i
    #return sum

    #2
    return sum([i ** i for i in range(1, x)])


def funcOptimized(x, digitCount):
    sum = 0
    digitCutter, _ = fastPow(10, digitCount)
    for i in range(1, x):
        tmp, _ = fastPow2(i, i, digitCount)
        sum += tmp
        sum %= digitCutter
    return sum


def getNotZeroBitPositions(num):
    b = 1
    c = 0
    powers = []
    while b <= num:
        tmp = num & b
        if tmp:
            powers.append(c)
        c += 1
        b <<= 1
    return list(reversed(powers))


def getNotZeroBitPositions2(num):
    powers = []
    tmp = num
    while tmp > 0:
        i = math.trunc(math.log(tmp, 2))
        powers.append(i)
        tmp -= 2**i
    return powers


def getNotZeroBitPositions3(num):
    bits = []
    tmp = num
    while tmp > 0:
        tmp, rem = divmod(tmp, 2)
        bits.append(rem)

    #1
    #powers = []
    #for i in range(len(bits)):
    #    if bits[i]:
    #        powers.append(i)
    #powers = list(reversed(powers))

    #2
    #powers2 = [i for i in range(len(bits)) if bits[i]]
    #powers2 = list(reversed(powers2))

    #3
    powers3 = [i for i in range(len(bits) - 1, -1, -1) if bits[i]]

    return powers3


def fastPowTest():
    base = 4
    power = 10
    num = base ** power

    num2, multCount = fastPow(base, power)

    print('{0} ** {1} = {2}'.format(base, power, num))
    print('optimized mult count = ' + str(multCount))

    if num != num2:
        print('fast pow res = {0}'.format(num2))


def fastPow(base, power):
    if power == 0:
        return (1, 0)

    powers = getNotZeroBitPositions(power)

    #checkAndVisualizeSplit(power, powers)

    multCount = 0
    highestPower = powers[0]
    lowPowers = powers[1:]
    powers_dict = { p : None for p in lowPowers }
    res = base
    for p in range(highestPower):
        if p in powers_dict:
            powers_dict[p] = res
        res *= res
        multCount += 1

    for p in lowPowers:
        res *= powers_dict[p]
        multCount += 1

    return (res, multCount)


def fastPow2(base, power, digitsCount):
    if power == 0:
        return (1, 0)

    digitCutter, _ = fastPow(10, digitsCount)

    powers = getNotZeroBitPositions(power)

    #checkAndVisualizeSplit(power, powers)

    multCount = 0
    highestPower = powers[0]
    lowPowers = powers[1:]
    powers_dict = { p : None for p in lowPowers }
    res = base % digitCutter
    for p in range(highestPower):
        if p in powers_dict:
            powers_dict[p] = res
        res *= res
        res %= digitCutter
        multCount += 1

    for p in lowPowers:
        res *= powers_dict[p]
        res %= digitCutter
        multCount += 1

    return (res, multCount)


def checkAndVisualizeSplit(power, powers):
    print('{0} = '.format(power), end='')

    #1
    #strings = []
    #for p in powers:
    #    strings.append('2^{0}'.format(p))
    #print(' + '.join(strings, ))

    #2
    print(' + '.join(['2^{0}'.format(p) for p in powers]))

    #combine split for check
    combinedPower = sum([2 ** p for p in powers])
    if combinedPower != power:
        print("CombinedPower != power ({0} != {1})".format(combinedPower, power))


def getLastDigits(num, count):
    x, _ = fastPow(10, count)
    return num % x


if __name__ == '__main__':
    main()
