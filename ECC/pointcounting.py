import random
def L(n, p):
    if n == 0:
        return True
    else:
        return pow(n, int((p-1)/2), p) == 1

    
def mod_sqrt(a, p):
    assert L(a, p)

    if a == 0:
        return 0

    # write p-1 = s*2^e for an odd s
    s = p - 1
    e = 0
    while s % 2 == 0:
        s /= 2
        e += 1
    s = int(s)

    # find a number that is not square mod p
    n = 2
    while L(n, p):
        n += 1

    x = pow(a, int((s + 1) / 2), p) # guess of square root
    b = pow(a, s, p) # "fudge factor" - loop invariant is x^2 = ab (mod p)
    g = pow(n, s, p) # used to update x and b
    r = e # exponent - decreases with each update

    while True:
        t = b
        m = 0
        for m in range(r):
            if t == 1:
                break
            t = pow(t, 2, p)

        if m == 0:
            return x

        gs = pow(g, 2 ** (r - m - 1), p)
        g = (gs * gs) % p
        x = (x * gs) % p
        b = (b * g) % p
        r = m

def mod_div(q,d,p):
    d_inv = pow(d, p - 2, p)

    return (q*d_inv) % p

def get_random_point(p,a,b):
    while True:
        x = random.randint(0, p-1)
        y_squared = (pow(x,3,p) + ((a*x) % p) + (b % p)) % p
        if L(y_squared, p):
            break
        
    y = mod_sqrt(y_squared, p)

    return (x,y)

def add_points(p,q,a,pr):
    if p == "inf":
        return q

    if q == "inf":
        return p

    if p == negate_point(q, pr):
        return "inf"
    if p == q:
        m = mod_div((3 * pow(p[0],2,pr) + a) % pr, (2 * p[1]) % pr, pr)
    else:
        m = mod_div((q[1] - p[1]) % pr, (q[0] - p[0]) % pr, pr)

    x = (-p[0] - q[0]) % pr + pow(m, 2, pr)
    y = m * (p[0] - x) - p[1]
    
    return (x % pr, y % pr)

def negate_point(pt, p):
    if pt == 'inf':
        return 'inf'
    
    return (pt[0], -pt[1] % p)

def bsgs(p,a,b):
    P = get_random_point(p,a,b)

    ms = set()

    baby_steps = {'inf': 0} 
    s = int(p**(1/4))
    next_multiple = 'inf'
    
    for i in range(1, s+1):
        next_multiple = add_points(next_multiple, P, a, p)
        baby_steps[next_multiple] = i
        baby_steps[negate_point(next_multiple, p)] = -i

    # the final value of next_multiple is sP
    two_sP = add_points(next_multiple, next_multiple, a, p)
    Q = add_points(two_sP, P, a, p) # Q = (2s + 1)P

    bin_str = bin(13+1)[2:]
    R = 'inf'
    doubled = P
    for bit in reversed(bin_str):
        if bit == '1':
            R = add_points(R, doubled, a, p)
        doubled = add_points(doubled, doubled, a, p)
    
    t = int(round((2 * p**(1/2)) / (2*s + 1))) # num giant steps--approx p^(1/4)
    
    next_Q_multiple = 'inf'

    for i in range(t+1):
        iQ = next_Q_multiple
        R_plus_iQ = add_points(R, iQ, a, p)
        if R_plus_iQ in baby_steps: # then R + iQ = jP
            j = baby_steps[R_plus_iQ]
            m = p + 1 + (2*s+1)*i - j # R + iQ - jP = mP = 0
            ms.add(m)

        R_minus_iQ = add_points(R, negate_point(iQ, p), a, p)
        if R_minus_iQ in baby_steps: # then R - iQ = jP
            j = baby_steps[R_minus_iQ]
            m = p + 1 + (2*s+1)*(-i) - j # R-iQ-jP = (p+1)P-i(2s+1)P-jP = mP = 0
            ms.add(m)

        next_Q_multiple = add_points(next_Q_multiple, Q, a, p)

    if len(ms) == 1:
        return ms.pop()
    else:
        return -1 # failure


def count_points(p,a,b):
    num_points = bsgs(p,a,b)
    twisted = False

    # find a non-square g
    for n in range(p):
        if not L(n,p):
            g = n
            break
    
    while num_points == -1:
        twisted = not twisted

        if twisted:
            twist_points = bsgs(p,(a * pow(g,2,p)) % p,(b * pow(g,3,p)) % p)

            if twist_points != -1:
                num_points = 2*(p+1) - twist_points # #E'(F_p)+#E(F_p) = 2(p+1)

        else:
            num_points = bsgs(p,a,b)

    return num_points

if __name__ == "__main__":
    import sys
    a = int(sys.argv[1])
    b = int(sys.argv[2])
    p = int(sys.argv[3])
    print(count_points(p,a,b))