#lang eopl

(define the-lexical-spec

  '((whitespace (whitespace) skip)

    (comment ("%" (arbno (not #\newline))) skip)

    (identifier

     (letter (arbno (or letter digit "_" "-" "?")))

     symbol)

    (number (digit (arbno digit)) number)

    (number ("-" digit (arbno digit)) number)
    (number ("+" digit (arbno digit)) number)
    ))



(define the-grammar

  '(

    (E (number) num-exp)
    (E ("-" "(" E "," E ")") diff-exp)
    (E ("+" "(" E "," E ")") add-exp)
    (E ("*" "(" E "," E")") mult-exp)
    (E ("/" "(" E "," E ")") div-exp)
    ))


(sllgen:make-define-datatypes the-lexical-spec the-grammar)

(define scan&parse

  (sllgen:make-string-parser the-lexical-spec the-grammar))

(define run
  (lambda (e) (value-of (scan&parse e))))



(define value-of
  (lambda (e)
    (cases E e
      (num-exp (n) n)
      (diff-exp (e1 e2) (- (value-of e1) (value-of e2)))
      (add-exp (e1 e2) (value-of (diff-exp e1 (diff-exp (num-exp 0) e2))))
      (mult-exp (e1 e2) (* (value-of e1) (value-of e2)))
      (div-exp (e1 e2) (/ (value-of e1) (value-of e2))))))



;Q2

(define example1 "+(-(4,2),+(*(3,3),2))")

  

(run example1)

(define example2 "/(/(/(8,2),2),2)")

  
(run example2)

