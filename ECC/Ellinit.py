import sys
import os
from ctypes import *

# load the library
pari=cdll.LoadLibrary(r"C:\Users\Tuanminh1910\AppData\Local\Pari64-2-15-1\libpari.dll")

pari.stoi.restype = POINTER(c_long)
pari.cgetg.restype = POINTER(POINTER(c_long))
pari.ellinit.restype = POINTER(POINTER(c_long))
pari.ellisdivisible.restype = c_long
pari.nfinit0.restype = POINTER(c_long)
pari.polcyclo_eval.restype = POINTER(c_long)
pari.pol_x.restype = POINTER(c_long)
pari.GENtostr.restype = c_char_p
pari.Fp_ellcard.restype = POINTER(c_long)
pari.Fp_ellcard.argtypes = [POINTER(c_char_p), POINTER(c_char_p), POINTER(c_char_p)]
pari.Fp_ellcard_SEA.restype = POINTER(c_long)
pari.Fp_ellcard_SEA.argtypes = [POINTER(c_char_p), POINTER(c_char_p), POINTER(c_char_p), c_long]
pari.gp_read_str.restype = c_char_p
pari.strtoi.restype = POINTER(c_char_p)
pari.genrand.restype = POINTER(c_char_p)
pari.getrand.restype = c_long
pari.setrand.restype = c_long

(t_VEC, t_COL, t_MAT) = (17, 18, 19)  # incomplete
precision = c_long(38)

pari.pari_init(2**32-1, 2**32-1)
def t_vec(numbers):
    l = len(numbers) + 1
    p1 = pari.cgetg(c_long(l), c_long(t_VEC))
    for i in range(1, l):
        p1[i] = pari.stoi(c_long(numbers[i - 1]))
    return p1

def main():
    os.system('cls')
    a = str(sys.argv[1]).encode('utf-8')
    b = str(sys.argv[2]).encode('utf-8')
    p = str(sys.argv[3]).encode('utf-8')
    res = pari.Fp_ellcard(pari.strtoi(a), pari.strtoi(b), pari.strtoi(p))
    h = (0, 0, 0, int(sys.argv[1]), int(sys.argv[2]))
    e = pari.ellinit(t_vec(h), pari.strtoi(p))
    P = pari.genrand(e)
    pari.pari_printf(bytes("%Ps\n", "utf8"), res)
    

if __name__ == '__main__':
    main()