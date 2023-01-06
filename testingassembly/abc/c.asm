.globl foo
.globl nothing
foo:
	addi $sp, $sp, -4
	sw $ra, 0($sp)
	jal nothing
	lw $ra, 0($sp)
	addi $sp, $sp, 4
	jr $ra
