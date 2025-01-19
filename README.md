# Calculator-A-ED1
Repositorio correspondiente al tercer proyecto de algoritmos y Estructuras de Datos 1.

## Evaluación de expresiones matemáticas
Este proyecto consiste en construir una calculadora que evalúa expresiones de longitud
arbitraria. Con ese fin se utilizará un árbol de expresión binaria. La calculadora realizará
operaciones algebraicas simples (+, -, *, /, %, **), así como operaciones lógicas (and, or,
not, xor) de cualquier longitud, colocando la expresión en un árbol de expresiones binarias
y luego evaluando el árbol de expresiones.
Un árbol de expresión binaria es un árbol binario, que tiene como máximo dos hijos.
Recuerde que existen dos tipos de nodos en un árbol binario, los nodos hoja que no tienen
hijos y los nodos internos que tienen uno o más hijos (y forman el cuerpo de el árbol). En un
árbol de expresión binaria, los nodos internos contendrán los operadores de la expresión (+,
-, *, /, %, etc). Los nodos hoja contendrán los operandos de la expresión (en nuestro caso,
valores enteros).

El algoritmo para construir un árbol de expresión requiere utilizar la notación postfija
(también conocida como notación polaca inversa, una versión modificada de una notación
matemática inventada por matemáticos polacos a principios del siglo XX). La notación de
sufijo se usa ampliamente en los círculos de computación porque las expresiones anotadas
en notación de sufijo son completamente inequívocas sin tener que recurrir a paréntesis

