# BluePhoenix
 Minecraft Command Blocks Compiler
# Language Syntax
##Variables
 Variables can be define with <type> <variable_name>
 The type must be in lower case even for structs type and enum.
 Variables will not be associated with any entity when compiled.
```int variable1
float variable2 = 1.0
 variable1 += 5
 variable2 *= 2
 variable2 = variable1 + 5
 ```
 Multiple variable can be declared on the same line:
 ```
 int v1,v2,v3
 ```
###Entity Variable
 Variables can be define with <TYPE> <variable_name> = [scoreboard type]
 The type must be in upper case even for structs type and enum.
 Variables will be associated with any entity when compiled (default to "@s").
 ```INT variable1
 FLOAT variable2 = dummy
 ```
##Conditions
 ```if (variable1 && variable2 == 0){
  variable1 ++
 }```
 ###Block && Blocks
  ```if (block(~ ~ ~ stone)){
  variable1 ++
 }```
  ```if (blocks(0 0 0 1 1 1 ~ ~ ~)){
  variable1 ++
 }```
