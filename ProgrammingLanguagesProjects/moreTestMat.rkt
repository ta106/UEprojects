#lang eopl

(define num
  (lambda (x)
    (if (eqv? x 0) '() (append (list #t) (num (- x 1))))))


(define get-val
  (lambda (lst)
    (if (null? lst) 0 (+ 1 (get-val (cdr lst))))))

(define is-zero?
  (lambda(lst)
    (if (null? lst) #t #f)))

(define succ
  (lambda (x)
    (append (list #t) x)))

(define pred
  (lambda (x) (cdr x)))


;( minus x y) ; x -y = x-1 - y-1 = x - 2 y -2 ... = ( (y - y)


(define minus
  (lambda (x y)
    (if (is-zero? y) x (minus (pred x) (pred y)))))

(define add
  (lambda (x y)
    (if (is-zero? y) x (minus (succ x) (pred y)))))




(define-datatype pal pal?
  (empty-word)
  (b-word)
  (a-word)
  (b-pal (word pal?) )
  (a-pal (word pal?)))

(define odd
  (lambda (p)
    (cases pal p
      (empty-word () #f)
      (a-word () #t)
      (b-word () #t)
      (a-pal (word) (odd word))
      (b-pal (word) (odd word)))))

(define fact
  (lambda (x)
    (if (zero? x) 1
        (* x (fact (- x 1))))))
(define fact-acc
  (lambda (x)
    (calc x 1)))

(define calc
  (lambda (x acc)
    (if (zero? x) acc (calc (- x 1) (* x acc)))))















