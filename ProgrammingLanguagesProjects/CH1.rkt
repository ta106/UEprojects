#lang eopl

(define dple
  (lambda (n x)
    (if (zero? n) '()
        (cons x (dple (- n 1) x)))))
(define invt
  (lambda (2-lst)
    (if (null? 2-lst) '()
        (cons (list (car(cdr(car 2-lst))) (car (car 2-lst))) (invt (cdr 2-lst))))))
(define down
  (lambda (lst)
    (if (null? lst) '()
        (cons (list (car lst)) (down (cdr lst))))))

(define swapper
  (lambda (s1 s2 slst)
    (if (null? slst) '()
        (if (eqv? (car slst) s1) (cons s2 (swapper s1 s2 (cdr slst)))
            (if (eqv? (car slst) s2) (cons s1 (swapper s1 s2 (cdr slst)))
                (cons (car slst) (swapper s1 s2 (cdr slst))))))))
(define swap
  (lambda (s1 s2 slst)
    (cond
      ( (null? slst) '())
      ( (eqv? (car slst) s1) (cons s2 (swap s1 s2 (cdr slst))))
      ( (eqv? (car slst) s2) (cons s1 (swap s1 s2 (cdr slst))))
      ( (not (symbol? (car slst))) (cons (swap s1 s2 (car slst)) (swap s1 s2 (cdr slst))))
      (else (cons (car slst) (swap s1 s2 (cdr slst)))))))

(define list-set
  (lambda (lst n x)
    (if (zero? n) (cons x (cdr lst))
        (cons (car lst) (list-set (cdr lst) (- n 1) x)))))

(define count-oc
  (lambda (s lst)
    (cond
      ( (null? lst) 0 )
      ( (eqv? (car lst) s) (+ 1 (count-oc s (cdr lst))) )
      ( (not (symbol? (car lst))) (+ (count-oc s (car lst)) (count-oc s (cdr lst))))
      (else (count-oc s (cdr lst))))))
      