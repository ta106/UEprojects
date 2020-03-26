#lang eopl

(define-datatype Binary-Tree Binary-Tree?
  (empty-tree)
  (leaf)
  (node (left Binary-Tree?)(right Binary-Tree?)))

(define height
  (lambda (tree)
    (cases Binary-Tree tree
      (empty-tree () -1)
      (leaf () 0)
      (node (left right) (max (+ 1 (height left)) (+ 1 (height right)))))))

(define num-leaves
  (lambda (tree)
    (cases Binary-Tree tree
      (empty-tree () 0)
      (leaf () 1)
      (node (left right) (+ (num-leaves left) (num-leaves right))))))

(define bt-parse
  (lambda (lst)
    (cond
      ( (eqv? lst 'empty-tree) (empty-tree))
      ( (eqv? lst 'leaf ) (leaf))
      ( (list? lst) (node (bt-parse (car lst)) (bt-parse (cadr lst)))))))

(define-datatype plist plist?
  (a-list (p pexp?)))
(define-datatype pexp pexp?
  (num (n number?) )
  (sub (exp1 pexp?) (exp2 pexp?)))
(define valu
  (lambda (lst)
    (cases plist lst
      (a-list (x) (value- x))
      )))
(define value-
  (lambda (exp)
    (cases pexp exp
      (num (n) n)
      (sub (exp1 exp2) (- (value- exp1) (value- exp2))))))
(define every?
  (lambda (func lst)
    ( if (null? lst) #t (if (func (car lst)) (every? func (cdr lst)) #f))))