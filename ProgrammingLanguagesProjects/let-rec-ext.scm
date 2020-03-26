;; Turki AlHarbi
;; LETREC lang
;; Extra cridit CS380
;; Version 3
;; 11/8/2017

#lang eopl


;;;;;;;;;;;;;;;; grammatical specification ;;;;;;;;;;;;;;;;


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

  '((program (expression) a-program)
    (expression (number) const-exp)
    (expression (identifier) var-exp)
    (expression ("-" "(" expression "," expression ")") diff-exp)
    (expression ("+" "(" expression "," expression ")") add-exp)
    (expression ("*" "(" expression "," expression ")") mult-exp)
    (expression ("less?" "(" expression "," expression ")") less?-exp)
    (expression ("greater?" "(" expression "," expression ")") greater?-exp)
    (expression ("QuoInt" "(" expression "," expression ")") QuoInt-exp)
    (expression ("RemInt" "(" expression "," expression ")") RemInt-exp)
    (expression ("equal?" "(" expression "," expression ")") equal?-exp)
    (expression ("zero?" "(" expression ")") zero?-exp)
    (expression ("if" expression "then" expression "else" expression) if-exp)
    (expression ("let"  (arbno identifier "=" expression) "in" expression) let-exp)
    (expression ("let*"  (arbno identifier "=" expression) "in" expression) let*-exp)
    (expression ("proc" "(" (separated-list identifier ",") ")" expression) proc-exp)
    (expression ("letrec" (arbno identifier "(" (separated-list identifier ",") ")" "=" expression) "in" expression) letrec-exp)
    (expression ("minus" "(" expression ")") minus-exp)
    (expression ("(" expression (arbno expression) ")") call-exp)

    ))

(sllgen:make-define-datatypes the-lexical-spec the-grammar)

;;;;;;;;;;;;;;;; sllgen boilerplate ;;;;;;;;;;;;;;;;
(define scan&parse
  (sllgen:make-string-parser the-lexical-spec the-grammar))
;;;;;;;;;;;;;; environnment ;;;;;;;;;;;;;;;;;



(define-datatype env env?
  (empty-env)
  (extend-env (var symbol?) (val expval?) (env env?))
  (extend-env-rec
   (p-names (list-of symbol?))
   (b-vars  (list-of (list-of symbol?)))
   (p-bodys (list-of expression?))
   (saved-env env?)))

(define apply-env
  (lambda (env1 var)
    (cases env env1
      (empty-env () (eopl:error 'apply-env "empty env ~s" var))
      (extend-env (var1 val exenv)
                  (if (eqv? var1 var) val (apply-env exenv var)
                     ))
      (extend-env-rec (p-names b-vars p-bodys saved-env)
                      (if (hasproc? var p-names) (findproc var p-names b-vars p-bodys env1)
                          (apply-env saved-env var)
                        )))))
(define hasproc?
  (lambda (var p-names) (if (null? p-names) #f (if (eqv? var (car p-names)) #t (hasproc? var (cdr p-names))))))

(define findproc
  (lambda (var p-names b-vars p-bodys env1)
    (if (null? p-names) (eopl:error 'findproc "no proc") (if (eqv? var (car p-names)) (proc-val (procedure (car b-vars) (car p-bodys) env1)) (findproc var (cdr p-names) (cdr b-vars) (cdr p-bodys) env1)))))

(define extend-mult-env
  (lambda (vars vals env)
    (if (null? vars) env (extend-env (car vars) (car vals) (extend-mult-env (cdr vars) (cdr vals) env))))) ; first is last

(define extend-*-env
  (lambda (vars vals env)
    (if (null? vars) env (extend-*-env (cdr vars) (cdr vals) (extend-env (car vars) (value-of (car vals) env) env) )))) ; 
;;;;;;;;;;;;;;;; proc ;;;;;;;;;;;;;;;;;;;;;;;;

(define-datatype proc proc?
  (procedure
   (vars (list-of symbol?)) (body expression?) (env1 env?)))

(define apply-procedure
  (lambda (proc1 vals)
    (cases proc proc1
      (procedure (vars body saved-env)
                 (if (eqv? (length vars) (length vals)) 
                 (value-of body (extend-mult-env vars vals saved-env)) (eopl:error 'apply-proceduer "arguments mismatch"))))))


;;;;;;;;;;;;;;;; DataTypes;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;


(define-datatype expval expval?
  (num-val
   (num number?))
  (bool-val
   (bool boolean?))
  (proc-val
   (proc proc?)))

(define expval->num
  (lambda (val)
    (cases expval val
      (num-val (num) num)
      (else (report-expval-extractor-error 'num val)))))


(define expval->bool
  (lambda (val)
    (cases expval val
      (bool-val (bool) bool)
      (else (report-expval-extractor-error 'bool val)))))

(define expval->proc
  (lambda (val)
    (cases expval val
      (proc-val (proc) proc) (else (eopl:error 'proc-val "not a proc")))))


(define report-expval-extractor-error
  (lambda (type val)
    (if (eqv? type 'num)
        (eopl:error "not a number") (eopl:error "not a bool"))))

(define run
 (lambda(string)
 (value-of-program (scan&parse string))))

(define value-of-program
  (lambda (pgm)
    (initialize-store!)
    (cases program pgm
      (a-program (exp1)
        (expval->num (value-of exp1 (init-env)))))))

(define init-env (lambda () (extend-env 'i (num-val 1) (extend-env 'v (num-val 5) (extend-env 'x (num-val 10) (empty-env))))))

(define value-of
  (lambda (exp env)
    (cases expression exp
      (const-exp (num) (num-val num))
      (var-exp (var)  (apply-env env var))
      (less?-exp (exp1 exp2)  (bool-val (< (expval->num (value-of exp1 env)) (expval->num (value-of exp2 env)))))
      (greater?-exp (exp1 exp2) (value-of (less?-exp exp2 exp1) env))
      (diff-exp (exp1 exp2)
                (num-val (- (expval->num (value-of exp1 env)) (expval->num (value-of exp2 env)))))
      (equal?-exp (exp1 exp2) (value-of (if-exp (less?-exp exp1 exp2) (less?-exp (const-exp 1) (const-exp 0)) (if-exp (less?-exp exp2 exp1) (less?-exp (const-exp 1) (const-exp 0)) (less?-exp (const-exp 0) (const-exp 1))))env))
      (zero?-exp (exp1)
                 (value-of (equal?-exp (const-exp 0) exp1) env))
      (if-exp (exp1 exp2 exp3)
              (if (expval->bool (value-of exp1 env)) (value-of exp2 env) (value-of exp3 env)))
      (minus-exp (exp)
                 (value-of (diff-exp (const-exp 0) exp) env))
      (add-exp (exp1 exp2)
       (value-of (diff-exp exp1  (minus-exp exp2) ) env))
      (mult-exp (exp1 exp2) ; letrec mult(x,y) = if zero?(y) then 0 else +( (mult x -(y,1)), x) in (mult exp1 exp2)
                (value-of (letrec-exp '(mult) '((x y)) (list (if-exp (zero?-exp (var-exp 'y)) (const-exp 0) (add-exp (call-exp (var-exp 'mult) (list (var-exp 'x) (diff-exp (var-exp 'y) (const-exp 1)))) (var-exp 'x))))
                                      (let-exp '(times)
                               (list (proc-exp '(x y)
                                  (if-exp
                                    (less?-exp (var-exp 'x) (const-exp 0))
                                    (if-exp
                                      (less?-exp (var-exp 'y) (const-exp 0))
                                      (call-exp (var-exp 'mult) (list (diff-exp (const-exp 0) (var-exp 'x)) (diff-exp (const-exp 0) (var-exp 'y))))
                                      (diff-exp (const-exp 0) (call-exp (var-exp 'mult) (list (diff-exp (const-exp 0) (var-exp 'x)) (var-exp 'y)))))
                                    (if-exp
                                      (less?-exp (var-exp 'y) (const-exp 0))
                                      (diff-exp (const-exp 0) (call-exp (var-exp 'mult) (list (var-exp 'x) (diff-exp (const-exp 0) (var-exp 'y)))))
                                      (call-exp (var-exp 'mult) (list (var-exp 'x) (var-exp 'y)))))))
                               (call-exp (var-exp 'times) (list exp1 exp2)))) env)) 

      (QuoInt-exp (exp1 exp2);letrec Quo(x,y) = if zero?(x) then 0 else if less?(x,0) then -1 else +( (Quo -(x,y) y), 1) in
                              ;let div = proc(x,y) if less?(x,0) then if less?(y,0) then (Quo -(0,x) -(0,y)) else -(0, (Quo -(0,x) y)) else if less?(y,0) then -( 0, (Quo x -(0,y)) ) else (Quo x y) in (div -5 -5)
                  (value-of (letrec-exp '(Quo) '((x y))
                                        (list (if-exp
                                               (zero?-exp (var-exp 'x))
                                               (const-exp 0)
                                               (if-exp
                                                (less?-exp (var-exp 'x) (const-exp 0))
                                                (const-exp -1)
                                                (add-exp (call-exp (var-exp 'Quo) (list (diff-exp (var-exp 'x) (var-exp 'y)) (var-exp 'y))) (const-exp 1)))))
                                        (let-exp '(div)
                               (list (proc-exp '(x y)
                                  (if-exp
                                    (less?-exp (var-exp 'x) (const-exp 0))
                                    (if-exp
                                      (less?-exp (var-exp 'y) (const-exp 0))
                                      (call-exp (var-exp 'Quo) (list (diff-exp (const-exp 0) (var-exp 'x)) (diff-exp (const-exp 0) (var-exp 'y))))
                                      (diff-exp (const-exp 0) (call-exp (var-exp 'Quo) (list (diff-exp (const-exp 0) (var-exp 'x)) (var-exp 'y)))))
                                    (if-exp
                                      (less?-exp (var-exp 'y) (const-exp 0))
                                      (diff-exp (const-exp 0) (call-exp (var-exp 'Quo) (list (var-exp 'x) (diff-exp (const-exp 0) (var-exp 'y)))))
                                      (call-exp (var-exp 'Quo) (list (var-exp 'x) (var-exp 'y)))))))
                               (call-exp (var-exp 'div) (list exp1 exp2)))) env))









                  ; (letrec-exp '(div) '((x y)) (list (if-exp (zero?-exp (var-exp 'x)) (const-exp 0) (if-exp (less?-exp (var-exp 'x) (const-exp 0)) (const-exp -1)

                   ;                                                      (add-exp (call-exp (var-exp 'div) (list (diff-exp (var-exp 'x) (var-exp 'y)) (var-exp'y))) (const-exp 1))))) (call-exp (var-exp 'div) (list exp1 exp2))) env))

      (RemInt-exp (exp1 exp2);letrec Rem(x,y) = if zero?(x) then 0 else if less?(x,0) then -1 else +( (Quo -(x,y) y), 1) in
                              ;let div = proc(x,y) if less?(x,0) then if less?(y,0) then (Rem -(0,x) -(0,y)) else -(0, (Quo -(0,x) y)) else if less?(y,0) then -( 0, (Quo x -(0,y)) ) else (Quo x y) in (div -5 -5)
                  (value-of (letrec-exp '(Rem) '((x y))
                                        (list (if-exp
                                               (zero?-exp (var-exp 'x))
                                               (const-exp 0)
                                               (if-exp
                                                (less?-exp (var-exp 'x) (const-exp 0))
                                                (add-exp (var-exp 'x) (var-exp 'y))
                                                (call-exp (var-exp 'Rem) (list (diff-exp (var-exp 'x) (var-exp 'y)) (var-exp 'y))) )))
                                        (call-exp (var-exp 'Rem) (list exp1 exp2))) env))

      (let-exp (vars exps body)
               (value-of body (extend-mult-env vars  (map (lambda (x) (value-of x env)) exps) env)))
      (let*-exp (vars exps body) (value-of body (extend-*-env vars exps env)))
      ;(begin-exp (exp exps) (letrec ( (value-of-each (lambda (ex exs)
      (proc-exp (vars body)
                (proc-val (procedure vars body env)))
  ;    (newref-exp (exp1) (let ((v1 (value-of exp1 env))) (ref-val (newref v1))))
   ;   (deref-exp (exp1) (let ((v1 (value-of exp1 env))) (let ((ref1 (expval->ref v1))) (deref ref1))))
    ;  (setref-exp (exp1 exp2) (let ((ref (expval->ref (value-of exp1 env)))) (let ((val2 (value-of exp2 env))) (begin (setref! ref val2) (num-val 23)))))
      (call-exp (rator rands)
                (apply-procedure (expval->proc (value-of rator env)) (map (lambda (x) (value-of x env)) rands)))
      (letrec-exp (p-names b-vars p-body letrec-body)
                  (value-of letrec-body
                            (extend-env-rec p-names b-vars p-body env))))))

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;; Store Stuff ;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
(define empty-store
  (lambda () '()))

(define the-store 'uninitialized)

(define get-store
  (lambda () the-store))

(define initialize-store!
  (lambda ()
    (set! the-store (empty-store))))

(define reference?
  (lambda (v)
    (integer? v)))

(define newref
  (lambda (val)
    (let ((next-ref (length the-store)))
      (set! the-store (append the-store (list val))) next-ref)))

(define deref
  (lambda (ref)
    (list-ref the-store ref)))

(define setref!
  (lambda (ref val)
    (set! the-store
          (letrec ((setref-inner
                    (lambda (store1 ref1)
                      (cond
                        ((null? store1)
                         (report-invalid-reference ref the-store))
                        ((zero? ref1) (cons val (cdr store1)))
                        (else (cons (car store1) (setref-inner (cdr store1) (- ref1 1))))))))
            (setref-inner the-store ref)))))

(define report-invalid-reference
  (lambda (ref the-store) (eopl:error 'report-invalid-reference "~s" ref "does not exist in ~s" the-store))) 



;;Tests
(run "let x =4 y=3 in -(x,y)");4-3=1
(run "let x =4 y=3 in +(x,y)");4+3=7
(run "let x =4 y=3 in *(x,y)");4*3=12
(run "let x =10 y=5 in RemInt(x,y)"); 10 % 5 =0
(run "let x =10 y=5 in QuoInt(x,y)");10/5 = 2
(run "let x =5 y=10 in RemInt(x,y)");5%10 = 5
(run "let x =5 y=10 in QuoInt(x,y)");5/10 = 0
(run "let x =4 y=3 in *(minus(x),y)");-4*3 = -12
(run "let x =4 y=3 in *(x,minus(y))");4*-3 = -12
(run "let x =4 y=3 in *(minus(x),minus(y))");-4*-3 = 12
(run "let x =10 y=5 in QuoInt(x,minus(y))"); 10/-5 = -2
(run "let x =10 y=5 in QuoInt(minus(x),y)"); -10/5 = -2
(run "let x =10 y=5 in QuoInt(minus(x),minus(y))");-10/-5 = 2
(run "let makemult = proc (maker) proc (x) if zero?(x) then 0 else -(((maker maker) -(x,1)), -4) in let times4 = proc (x) ((makemult makemult) x) in (times4 3)") ; 4 * 3 = 12
(run " letrec factorial (n) = if zero?(n) then 1 else *(n, (factorial -(n,1))) in (factorial 5)"); 5! = 120
(run "let makerec = proc (f) let d = proc (x) proc (z) ((f (x x)) z) in proc (n) ((f (d d)) n) in let maketimes4 = proc (f) proc (x) if zero?(x) then 0 else -((f -(x,1)), -4) in let times4 = (makerec maketimes4) in (times4 3)");4*3 =12
(run "let x = 30 in let x = -(x,1) y = -(x,2) in -(x,y)");x = 30 ;x=29 ; y =28 ; x - y = 29 - 28 = 1
(run "let x = 30 in let* x = -(x,1) y = -(x,2) in -(x,y)");x = 30 ;x=29 ; y =27 ; x - y = 29 - 27 = 2
(run "letrec even(x) = if zero?(x) then 1 else (odd -(x,1)) odd(x) = if zero?(x) then 0 else (even -(x,1)) in (odd 13)")
(run "let o= proc() 3 in (o )")



