#lang eopl

(define len-entries
  (lambda (lst)
  (entr lst 1)))

(define entr
  (lambda (lst rec-num)
    (if (null? lst) '()
        (cons rec-num (entr (cdr lst) (+ 1 rec-num))))))


(define merge
  (lambda (lst lst1)
    (if (null? lst) lst1
        (if (null? lst1) lst
              (cons (car lst) (cons (car lst1) (merge (cdr lst) (cdr lst1))))))))
