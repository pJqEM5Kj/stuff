#import


def main():
    print('exec main from module: {0}'.format(__name__))
    print()

    #------------------------------------------------------------

    #fractionTest()

    problem()

    #
    pass


def problem():

    divTests = []

    for i in range(2, 1000):
        divTests.append((1, i))

    #r_end = 400
    #for i in range(1, r_end):
    #    for j in range(1, r_end):
    #        divTests.append((i, j))

    #special tests
    divTests.append((5, 2))
    divTests.append((10, 2))
    divTests.append((31, 2))
    divTests.append((301, 2))
    divTests.append((3001, 2))
    divTests.append((300, 2))
    divTests.append((300, 299))
    divTests.append((3000, 299))
    divTests.append((30000, 299))
    divTests.append((300000, 299))

    longest = None

    for i in divTests:
        n = i[0]
        d = i[1]

        z = n / d
        if float.is_integer(z):
            z = int(z)

        f = divv(n, d)

        if longest == None or longest[0] < len(f[2]):
            longest = len(f[2]), n, d #, f

        z_str = str(z)
        f_str = toStr(*f)
        f_str2 = toStr2(*f)

        showOnlyIfDiff = True

        compareDigitCount = 15
        isDiff = (z_str[:compareDigitCount] != f_str2[:compareDigitCount])

        if (showOnlyIfDiff and isDiff) or (not showOnlyIfDiff):
            print('({0} / {1}) = {2}'.format(n, d, z))
            if isDiff:
                print('diff')
            print(f_str)
            print(f_str2)
            print('-----------------------------------------------')

    print('done')
    print(longest)
    pass


def divv(nominator, denominator):
    if denominator == 0:
        raise Exception('div by zero error')

    if nominator == 0:
        return [0], [], []

    if denominator == 1:
        return [nominator], [], []

    nom_digits_stack = []
    num = nominator
    while num != 0:
        num, r = divmod(num, 10)
        nom_digits_stack.append(r)

    num = nom_digits_stack.pop()
    d1 = []

    while True:
        wasDigitAdded = False
        while num < denominator:
            if len(nom_digits_stack) > 0:
                dig = nom_digits_stack.pop()
                num = num * 10 + dig

                if wasDigitAdded:
                    d1.append(0)
                else:
                    wasDigitAdded = True
            else:
                if wasDigitAdded:
                    d1.append(0)
                break

        if num < denominator:
            break

        q, r = divmod(num, denominator)
        d1.append(q)
        num = r

    if len(d1) == 0:
        d1.append(0)

    while len(d1) > 1 and d1[0] == 0:
        d1.pop(0)

    d2 = []
    d3 = []

    if num == 0:
        return d1, d2, d3

    remainders = {}

    while True:
        wasDigitAdded = False
        while num < denominator:
            num_orig = num
            num *= 10
            if wasDigitAdded:
                d2.append(0)
            else:
                wasDigitAdded = True
            remainders[num_orig] = len(d2)

        q, r = divmod(num, denominator)
        d2.append(q)

        if r == 0:
            break

        if r in remainders:
            indx = remainders[r]
            while indx < len(d2):
                d3.append(d2.pop())
            d3 = list(reversed(d3))
            break

        num = r


    return d1, d2, d3


def toStr(d1, d2, d3):
    _str = ''
    for i in d1:
        _str += str(i)

    if len(d2) > 0 or len(d3) > 0:
        _str += '.'

    if len(d2) > 0:
        for i in d2:
            _str += str(i)

    if len(d3) > 0:
        _str += '('
        for i in d3:
            _str += str(i)
        _str += ')'
    return _str


def toStr2(d1, d2, d3, size = 30):
    _str = ''
    for i in d1:
        _str += str(i)

    if len(d2) > 0 or len(d3) > 0:
        _str += '.'

    if len(d2) > 0:
        for i in d2:
            _str += str(i)

    if len(d3) > 0:
        while len(_str) < size:
            for i in d3:
                if len(_str) == size:
                    break
                _str += str(i)

    return _str


if __name__ == '__main__':
    main()
