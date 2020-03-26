#lang eopl

(define sum-list
  (lambda (lst)
    (if (null? lst) 0
        (+ (car lst) (sum-list (cdr lst))))))

