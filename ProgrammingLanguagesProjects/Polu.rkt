#lang eopl
(define-datatype polynomial polynomial?
  (empty-poly)
  (poly-exp (cof number?) (exp integer?) (poly polynomial?)))

(define degree
  (lambda (poly)
    (cases polynomial poly
      (empty-poly () 0)
      (poly-exp (cof exp saved-poly) (largest exp (degree saved-poly))))))

(define largest
  (lambda(x y)
    (if (> x y) x y)))

(define greatest
  (lambda (poly)
    (cases polynomial poly
      (empty-poly () 0)
      (poly-exp (cof exp poly1) (largest cof (greatest poly1))))))


(define exponentiate
  (lambda (cf exp)
    (if (zero? exp) 1
    (if (eqv? exp 1) cf
        (* cf (exponentiate cf (- exp 1)))))))

(define eval
  (lambda (num poly)
    (cases polynomial poly
      (empty-poly () 0)
      (poly-exp (cf exo saved-poly) (+ (* cf (exponentiate num exo)) (eval num saved-poly))))))

(define add-poly
  (lambda (poly1 poly2)
    (cases polynomial poly1
      (empty-poly () poly2)
      (poly-exp (cf exp saved-poly) (poly-exp cf exp (add-poly saved-poly poly2))))))

(define derivative
  (lambda (poly)
    (cases polynomial poly
      (empty-poly () (empty-poly))
      (poly-exp (cf exp poly1)
                (if (zero? exp) (derivative poly1) (poly-exp (* cf exp) (- exp 1) (derivative poly1)))))))

                                 

(define-datatype term term?
  (first-term (cf number?) (exp integer?))
  (plus-term (cf number?) (exp integer?))
  (minus-term (cf number?) (exp integer?)))


(define cons-poly
  (lambda(lst)
    (if (null? lst) (empty-poly)
        (cases term (car lst)
          (first-term (cf exp) (poly-exp cf exp (cons-poly (cdr lst))))
          (plus-term (cf exp) (poly-exp cf exp (cons-poly (cdr lst))))
          (minus-term (cd exp) (poly-exp (- 0 cd) exp (cons-poly (cdr lst))))))))


(define the-lexical-spec

  '((whitespace (whitespace) skip)

    (comment ("%" (arbno (not #\newline))) skip)

    (identifier

     (letter (arbno (or letter digit "_" "-" "?")))

     string)

    (number (digit (arbno digit)) number)

    (number ("-" digit (arbno digit)) number)
    
    (number ("+" digit (arbno digit)) number)

    ))



(define the-grammar

  '((polynomial (number "x^" number polynomial) poly-exp)
    (polynomial () empty-poly)
    ))


;;;;;;;;;;;;;;;; sllgen boilerplate ;;;;;;;;;;;;;;;;


(define scan&parse

  (sllgen:make-string-parser the-lexical-spec the-grammar))



; (define get-number
 ;   (lambda (string)
  ;    (if (or (string=? string "+") (or (string=? "-") (string=? ""))) 0 (+ 1 (get-number (substring string 1 (string-length string)))))))
;(define parse
 ; (lambda (string)
  ;  (if (string=? string "") empty-poly)
   ; (if (string?= (substring string 0 1) "x")
    ;    (if (string?= (substring string 1 2) "^")
     ;       (poly-exp 1 (string->number (substring string 2 (get-number (substring string 2 (string-length string))))) (parse (substring string 3 (string-length string))))
      ;      (poly-exp 1 1 (parse (substring string 1 (string-length string)))))
       ; (if  (string?= (substring string 0 1) "+")
        ;     (
         