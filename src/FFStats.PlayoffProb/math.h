#ifndef MATH_H
#define MATH_H

namespace {
  constexpr unsigned int _pow(const unsigned int base, const unsigned int exp) {
    return exp == 0 ? 1 : base * _pow(base, exp - 1);
  }
}

#endif
