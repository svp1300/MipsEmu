.globl main

main:
    li $t0, 8
    move $t1, $zero
sum_loop:
    beq $t0, $zero, sum_done
    addi $t0, $t0, -1
    addi $t1, $t1, 2
    j sum_loop
sum_done:
    li $v0, 1
    move $a0, $t1
    syscall
    jr $ra