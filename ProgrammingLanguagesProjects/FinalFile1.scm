#lang eopl

(define-datatype E E?
  (num-exp (n number?))
  (add-exp (exp1 E?) (exp2 E?))
  (diff-exp (exp1 E?) (exp2 E?))
  (mult-exp (exp1 E?) (exp2 E?))
  (div-exp (exp1 E?) (exp2 E?)))



(define value-of
  (lambda (e)
    (cases E e
      (num-exp (n) n)
      (diff-exp (e1 e2) (- (value-of e1) (value-of e2)))
      (add-exp (e1 e2) (value-of (diff-exp e1 (diff-exp (num-exp 0) e2))))
      (mult-exp (e1 e2) (* (value-of e1) (value-of e2)))
      (div-exp (e1 e2) (/ (value-of e1) (value-of e2))))))



;Q2

(define example1
  (add-exp (diff-exp (num-exp 4) (num-exp 2)) (add-exp (mult-exp (num-exp 3) (num-exp 3)) (num-exp 2))))

(value-of example1)

(define example2
  (div-exp (div-exp (div-exp (num-exp 8) (num-exp 2)) (num-exp 2)) (num-exp 2)))

(value-of example2)

