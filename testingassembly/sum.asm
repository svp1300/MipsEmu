	.data
	.align 2
array:
	.space 40
invalid_input:
	.asciiz "Input must be greater than zero and less than "
sum_text:
	.ascii "Sum of inputs: "
new_line:
	.asciiz "\n"
line_end:
	.asciiz ".\n"
space:
	.asciiz " "
number_prompt_string:
	.asciiz "Enter a number: "

	.text
	.align 2 

	.globl main

main:
	addi $sp, $sp, -4
	sw $ra, 0($sp)
	jal read_array	
	move $a0, $v0
	jal sum_array
	lw $ra, 0($sp)
	addi $sp, $sp, 4	
	jr $ra

sum_array:
	move $t2, $a0

	add $t0, $zero, $zero		
	la $t1, array			
	add $t2, $t2, $t1		
sum_array_loop:
	beq $t1, $t2, sum_array_done	
	lw $t3, 0($t1)			
	add $t0, $t0, $t3		
	addi $t1, $t1, 4		
	j sum_array_loop 
sum_array_done:
	li $v0, 4
	la $a0, sum_text
	syscall				
	li $v0, 1
	move $a0, $t0
	syscall				
	li $v0, 4
	la $a0, new_line
	syscall				

	jr $ra

read_array:
	addi $sp, $sp, -12
	sw $s0, 0($sp)			
	sw $s1, 4($sp)
	sw $ra, 8($sp)
	add $s0, $zero, $zero		
	la $s1, array			
read_array_loop:
	la $a0, number_prompt_string
	li $a1, 4
	jal print			
	jal readnumber			
	
	li $t1, 7000		
	beq $v0, $t1, read_array_done	

	# check input
	li $t2, 100		
	slt $t3, $v0, $t2		
	beq $t3, $zero, read_array_error

	slt $t3, $v0, $zero		
	bne $t3, $zero, read_array_error
	
	# number is valid! store it.
	add $t1, $s0, $s1
	sw $v0, 0($t1)			

	addi $s0, $s0, 4		

	li $t0, 40
	beq $s0, $t0, read_array_done	

	j read_array_loop
read_array_error:
	la $a0, invalid_input
	li $a1, 4
	jal print			
	li $a0, 100
	li $a1, 1
	jal print			
	la $a0, line_end
	li $a1, 4
	jal print
	j read_array_loop		
read_array_done:
	move $v0, $s0			
	lw $ra, 8($sp)
	lw $s1, 4($sp)			
	lw $s0, 0($sp)
	addi $sp, $sp, 12
	jr $ra
	
	
# a0 - what to print
# a1 - what is printed
print:
	addi $sp, $sp, -4
	sw $v0, 0($sp)			

	# print stuff
	move $v0, $a1
	syscall

	lw $v0, 0($sp)			
	addi $sp, $sp, 4
	jr $ra


readnumber:
	li $v0, 5
	syscall
	jr $ra	


