.globl main
.globl foo

main:
	addi $sp, $sp, -4
	sw $ra, 0($sp)
	jal foo
	lw $ra, 0($sp)
	addi $sp, $sp, 4
	jr $ra
