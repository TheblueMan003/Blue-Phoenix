# BluePhoenix
 Minecraft Command Blocks Compiler
# Language Syntax
## Variables
 Variables can be define with 
 ```
 <type> <variable_name>
 ```
 The type must be in lower case even for structs type and enum.
 Variables will not be associated with any entity when compiled.
 ```
 int variable1
 float variable2 = 1.0
 const float pi = 3.14
 variable1 += 5
 variable2 *= 2
 variable2 = variable1 + 5
 ```
 Multiple variable can be declared on the same line:
 ```
 int v1,v2,v3
 ```
 Default avaible type: int, float, string, bool, function
### Entity Variable
 Variables can be define with 
 ```
 <TYPE> <variable_name> = [scoreboard type]
 ```
 The type must be in upper case even for structs type and enum.
 Variables will be associated with any entity when compiled (default to "@s").
 ```
 INT variable1
 FLOAT variable2 = dummy
 ```
## MCC
 If you want to insert a minecraft command inside you code you can simply put a "/" befor it
```
 /say hi
```
 Note: // define a comments.
 
## Conditions
 ```
 if (variable1 && variable2 == 0){
     variable1 ++
 }
 ```
 ### Block && Blocks
  ```
 if (block(~ ~ ~ stone)){
     variable1 ++
 }
 if (blocks(0 0 0 1 1 1 ~ ~ ~)){
     variable1 ++
 }
 ```
 ### Else & Else If
  ```
 if (condition1){
     variable1 ++
 }
 else if (condition2){
     variable2 ++
 }
 else{
     variable3 ++
 }
 ```
 ## Switch Statement
 Multiple if can be simplify as:
 ```
 switch(variable){
  0 -> /tellraw(@a,"action1")
  1 -> /tellraw(@a,"action2")
  case(2){
     /tellraw(@a,"action3")
  }
 }
 ```
 ## Loops
 The for and while loop can be define as follow:
 ```
 for(int i = 0;i < 10;i++){
                           
 }
 while(condition){
 
 }
 Note: Nested Loops are supported.
 ```
 ## Enum
 Enums can be used to create a new type. Variable that contains enums are represented as int
 ```
 enum direction = up, down, left, right
 enum example{
  val1,
  val2
 }
 direction dir = up
```
Enums can also have a list of parameters
```
 enum direction(int x, int y) = up(0,1), down(0,-1), left(-1,0), right(1,0)
 enum example(int x, int z = 0){
    val1(0), 
    val2(0)
 }
 direction dir = up
 int x = dir.x
```
Enums also can be define from resource files.
```
enums("enumName","file.txt","INIT")
enums("enumName","file.txt","CSV")
```
Supported files type are init and csv.
In a CSV enum, the first line of the file will be used parameters name. The first column is always the values names.
Types are infered by the compiler. If not type is found json is choosen.

 ## Function
 Functions can be created with one of those syntaxes:
```
  def name(){
 
  }
  void name(){
 
  }
```
 
 Functions can also take arguments && have return types:
 ```
  def name(int arg1, float arg2):int{
     return(0)
  }
  def name(int arg1, float arg2):int,int{
     return(0,0)
  }
  int name(int arg1, float arg2){
     return(0)
  }
 ```
 
 Functions can be put inside variables of type functions:
 ```
  def name(){
     return(0)
  }
  function func = name
  func()
  
  def name(int arg1, float arg2):int,int{
      return(0,0)
  }
  function<(int,float),(int,int)> func2 = name
  a,b = func2(0,0)
 ```
A sort notation also exist:
```
int=>void func2 = name
```

 Private Functions can only be used in the same namespace. By default all function are public.
 ```
 def private example(){
 }
 private void example(){
 }
 ```
 Functions can also be abstract allowing to call a function befor its created.
 ```
 def abstract example()
 example()
 def example(){
 
 }
 ```
 Note: Function overloading is possible and function are cap insensitive unlike variable.
 Functions only support tailrecurtion (No Function Stack yet)

 ### Lazy Functions
 Lazy functions are function that are evaluation by substitution (during compiling time) meaning that calling one will simply replace it with it body.
 ```
 def lazy example(){
    tellraw(@a,"test")
 }
 example()
 ```
 Lazy functions can also have a return type & argument. Argument will be replaced in the body with passed argument.
 If an argument start with a $ it will be replaced everywhere. (Even in text)
 If the type of a such argument is
  int: Will be replace by it value without space befor and after
  string: Will be replace by it value with space befor and after
  function: Will be replace by function name instead of function as lambda.
 ```
 def lazy say(int $text){
    /say $text
 }
 ```
 Lazy functions can have special argument types such as json && params. Those must always be the last arguments.
 ```
 def lazy show(json $text){
     tellraw(@a,$text)
 }
 show("test",("test",red),("test",yellow))
 ```
 Note: Lazy functions do not support recursion.
 
 ### Anonymous Functions
 Functions that have function as argument can be call in that manner:
 ```
 def decorator(function func){
    /say start
    func()
    /say end
 }
 decorator(){
    /say body
 }
 ```
Another way of defining anonymous functions is:
```
 decorator(=>/say body)
```
 
 ### Functions tags
 Functions can have tags define as bellow. Tags create a new function that contains all function that have the same tags.
 ```
 def @test test(){
 
 }
 @test()
 ```
 Functions can have multiple tags. At function can be call before a function get this tags.
 There are three special tags that do not start with '@': ticking, loading, helper
  ticking function will be executed each tick.
  loading function will be executed when the datapack is reload
  helper function will be always be present inside the datapack even if note used. (Useful for library.)

 ## Jsonfile
 Json files like craft file can be created with this construction:
 ```
   jsonfile recipes/custom_craft{
		<json_content>
   }
 ```
Forgenerating loop and lazy function can be used inside and outside of jsonfile.

## Predicates
Predicate can be create like:
```
predicate overworld(){
	"condition": "minecraft:location_check",
	"predicate": {
		"dimension": "minecraft:overworld"
	}
}
```
Argument can put inside the parenthis:
```
predicate inDimension($name){
	"condition": "minecraft:location_check",
	"predicate": {
		"dimension": "$name"
	}
}
```
Calling a predication in a condition can be done like a function.
 
 
 ## Package
 By default each file create a seperate package. If you want two files to by in the same package you can add this at the begin of the files.
 ```
 package name
 ```
 Note: If this is used in the middle of the code it will split the file into two package.
 
 This package '.' refer as the root pacakge each functions/variables inside this package will be accesible everwhere.
 
 ## Forgenerate
 Forgenerate is a loop evaluation during compile time meaning that everything inside of it will be duplicated as many time as needed.
 For Instance this:
 ```
 forgenerate($i,0,10){
    tellraw(@a,"$i")
 }
 ```
 Will become this:
 ```
 tellraw(@a,"0")
 tellraw(@a,"1")
 ...
 tellraw(@a,"10")
 ```
 Forgenerate can also be call with enums as second parameters.
 ```
 enum example = val1, val2
 forgenerate($i,example){
     tellraw(@a,"$i.index / $i.length -> $i")
 }
 ```
 Will be compiled to
 ```
 tellraw(@a,"0 / 2 -> val1")
 tellraw(@a,"1 / 2 -> val2")
 ```
 Note: Parameters in enums will also be replaced.
 
 ## Blocktags/Itemtags/Entitytags
 Minecraft Blocks/Entity/Item tags can be define the same as an enums but with the name blocktags instead of enums.
 ```
 blocktags cool_wool = white_wool, black_wool
 if (block(~ ~ ~ #cool_wool)){...}
 ```
 
 ## Struct
 Struct are another way to create new types. Struct can have variable and function inside of them.
 ```
 struct vector{
   int x,y,z
   def __init__(int x, int y, int z){
      this.x = x
      this.y = y
      this.z = z
   }
   int sum(){
      return(x,y,z)
   }
 }
 ```
 The constructor of a struct is the function call __init__.
 Struct also support operator overrloading with the function: __add__, __mult__, __div__, __sub__, __mod__
 Struct can also have lazy function.
 To instantiate a struct:
 ```
 vector vec = vector(0,0,0)
 ```
 ### Generic Struct
 Struct also support generic type.
 ```
 struct cell<X>{
   X val
   def set(X val){
     this.val = val
   }
   def get():X{
     return(val)
   }
 }
 ```
 Note: inside a struct function can also be static.

 ## Class
 Class are another way to create new types. Class can have variable and function inside of them.
 ```
 class vector{
   int x,y,z
   def __init__(int x, int y, int z){
      this.x = x
      this.y = y
      this.z = z
   }
   int sum(){
      return(x,y,z)
   }
 }
 ```
 The constructor of a class is the function call __init__.
 Class also support operator overrloading with the function: __add__, __mult__, __div__, __sub__, __mod__, __set__
 __set__ can only be overloader with other type then the current.
 Class can also have lazy function.
 To instantiate a class:
 ```
 vector vec = vector(0,0,0)
 ```
 ### Function Overriding
 Function inside a class can be marker as virtual. So class can then be override by function marked with override.
```
class example{
   ...
   def virutal function(){
      ...
   }
   ...
}
class example2 extends example{
   ...
   def override function(){
      ...
   }
   ...
}
```
### Inniter
Class are associated with an entity. By defaut all class extends the default object class and are thus area effect cloud. Inniter are function that will init the class by creatign the entity.
```
def armor_stand_initer(){
	/summon armor_stand ~ ~ ~ {Tags:["__class__","cls_trg"]}
	with(@e[tag=cls_trg]){
		untag(cls_trg)
		__CLASS__ = __class__
		__ref++
	}
}
class example inniter armor_stand_initer{
   ...
}
```

## Compiler Variable
Every variable that start with '$' will be treated as compiler variable. Each of it occurence will litteraly be replaced by it.
```
int $i = 0
print("$i")
```
Compiler variable can fetch from enum et const.
```
int $i = fromenum(enum, elem1)
int $j = fromconst(con)
```