;Turki AlHarbi
#lang eopl

(define natural?
  (lambda (num)
    (if (> 0 num) #f #t)))

(define-datatype polynomial polynomial?
  (num-poly (num integer?) )
  (full-poly (cf integer?) (exp natural?))
  (poly-poly (p1 polynomial?) (p2 polynomial?)))

(define construct-poly
  (lambda (lst)
    (cons-helper (car lst) (cadr lst))))

(define cons-helper
  (lambda (cf exp)
    (if (null? cf) (num-poly 0) (if (zero? (car cf)) (cons-helper (cdr cf) (cdr exp)) (poly-poly (full-poly (car cf) (car exp)) (cons-helper (cdr cf) (cdr exp)))))))
(define dx
  (lambda (poly)
    (cases polynomial poly
      (num-poly (num) (num-poly 0))
      (full-poly (cf exp) (if (zero? exp) (full-poly 0 0) (full-poly (* cf exp) (- exp 1))))
      (poly-poly (p1 p2) (poly-poly (dx p1) (dx p2))))))

(define exponentiate
  (lambda (cf exp)
    (if (zero? exp) 1
    (if (eqv? exp 1) cf
        (* cf (exponentiate cf (- exp 1)))))))

(define value-of
  (lambda (poly val)
    (cases polynomial poly
      (num-poly (num) num)
      (full-poly (cf exp) (* cf (exponentiate val exp)))
      (poly-poly (p1 p2) (+ (value-of p1 val) (value-of p2 val))))))

(define make-concrete
  (lambda (poly)
    (list  (find-cf poly) (find-exp poly))))
(define find-cf
  (lambda (poly)
    (cases polynomial poly
      (num-poly (num) (if (zero? num) '() num))
      (full-poly (cf exp) cf)
      (poly-poly (p1 p2)  (cons (find-cf p1) (find-cf p2))))))


(define find-exp
  (lambda (poly)
    (cases polynomial poly
      (num-poly (num) (if (zero? num)  '() num))
      (full-poly (cf exp) exp)
      (poly-poly (p1 p2) (cons (find-exp p1) (find-exp p2))))))
                           