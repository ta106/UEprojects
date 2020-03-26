#lang eopl

(define-datatype bintree bintree?
  (leaf-node (num integer?))
  (interior-node (key symbol?) (left bintree?) (right bintree?)))
(define b-t
  (lambda (bin)
    (cases bintree bin
      (leaf-node (num) (list 'leaf-node num))
      (interior-node (key left right) (list 'interior-node key (b-t left) (b-t right))))))

(define max-interior-h
  (lambda (tree)
    (cases bintree tree
      (leaf-node (num) (eopl:error 'leaf-node "can't get max of leaf"))
      (interior-node (s l r)
                     (cases bintree l
                       (leaf-node (num)
                                  (cases bintree r
                                    (leaf-node (num2) (result (+ num num2) s))
                                    (interior-node (sr rl rr) (b-result (result num s) (max-interior-h r)))))
                       (interior-node (sl ll lr)
                                      (cases bintree r
                                        (leaf-node (num2) (b-result (result num2 s) (max-interior-h l)))
                                        (interior-node (sr rl rr) (b-result (max-interior-h l) (max-interior-h r))))))))))
(define-datatype res res?
  (result (sum integer?) (s symbol?)))

(define-datatype rb-tree rb-tree?
  (red-bule-tree (rbs red-blue-subtree?)))
(define-datatype red-blue-subtree red-blue-subtree?
  (red-node (

(define b-result
  (lambda (res1 res2)
    (cases res res1
      (result (n s) (cases res res2
                      (result (n1 s1) (if (> n n1) res1 res2)))))))
(define max-int
  (lambda (tree)
    (r->sym (max-interior-h tree))))
(define r->sym
  (lambda (res1)
    (cases res res1
      (result (n s ) s))))