.globl main
.globl nothing
.globl foo

main:
	addi $sp, $sp, -32
	sw $ra, 0($sp)
	jal foo
	lw $ra, 0($sp)
	addi $sp, $sp, 32
	jr $ra
