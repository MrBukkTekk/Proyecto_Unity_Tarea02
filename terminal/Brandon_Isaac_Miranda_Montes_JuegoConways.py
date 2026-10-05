import sys
import msvcrt
import time

simboloJuga = "°"
simbolosuelo = "█"

cuadrote = [[" " for _ in range(80)] for _ in range(80)]

jugadorFil = 40
jugadorCol = 40

def dibujarTitulo():
    print("JUEGO DE LA VIDA DE CONWAY WUUUUUUUU")
    print("Hecho por Brandon Isaac Miranda Montes (dale q para iniciar la simulacion, debes hacer mas grande la terminal...)")
    print("-" * 80)

def cls_mover_cursor(fila, columna):
    cls_imprimir(f"\033[{fila};{columna}H")

def cls_imprimir(texto):
    sys.stdout.write(texto)
    sys.stdout.flush()

def cls_ocultar_cursor():
    cls_imprimir("\033[?25l")

def cls_mostrar_cursor():
    cls_imprimir("\033[?25h")

def limpiar():
    sys.stdout.write("\033[2J\033[H")
    sys.stdout.flush()

def dibujarPosi(fila, colu, simbolo):
    cls_mover_cursor(fila, colu)
    sys.stdout.write(simbolo)
    sys.stdout.flush()

def cls_leer_tecla():
    if not msvcrt.kbhit(): 
        return "NADA"
        
    tecla = msvcrt.getch()

    if tecla == b'\r':
        return "ENTER"

    if tecla in (b'\x00', b'\xe0'):
        flecha = msvcrt.getch()
        if flecha == b'H': return "ARRIBA"
        if flecha == b'P': return "ABAJO"
        if flecha == b'M': return "DERECHA"
        if flecha == b'K': return "IZQUIERDA"

    if tecla.lower() == b'q' or tecla == b'\x1b':
        return "SALIR"

    return "OTRA"

def contarCel(ma, fi, co):
    vecinosVivos = 0

    for u in range(fi-1, fi+2):
        for v in range(co-1,co+2):
            if u >= 0 and u < 80 and v >= 0 and v < 80:
                if not (u == fi and v == co):
                    if ma[u][v] == simbolosuelo:
                        vecinosVivos += 1
    return vecinosVivos

limpiar()
cls_ocultar_cursor()
dibujarTitulo()

dibujarPosi(jugadorFil + 4, jugadorCol + 1, simboloJuga)

while True: 
    mov = cls_leer_tecla()

    if mov == "NADA":
        continue

    if mov == "SALIR":
        break

    #imprime al final lo que sea que esté guardado, una viva o una muerta
    dibujarPosi(jugadorFil + 4, jugadorCol + 1, cuadrote[jugadorFil][jugadorCol])

    # pa no salir del 80x80
    if mov == "ARRIBA" and jugadorFil > 0:
        jugadorFil -= 1
    elif mov == "ABAJO" and jugadorFil < 79:
        jugadorFil += 1
    elif mov == "IZQUIERDA" and jugadorCol > 0: 
        jugadorCol -= 1
    elif mov == "DERECHA" and jugadorCol < 79:
        jugadorCol += 1
    
    # aca plantamos o quitamos una celula jeje
    elif mov == "ENTER":
        if cuadrote[jugadorFil][jugadorCol] == " ":
            cuadrote[jugadorFil][jugadorCol] = simbolosuelo
        else:
            cuadrote[jugadorFil][jugadorCol] = " "

    dibujarPosi(jugadorFil + 4, jugadorCol + 1, simboloJuga)

limpiar()
cls_mostrar_cursor()
print("BIEN, terminaste de elegir tus celulas vivas, no?\n Ahora vamos a ver que pasa...")
print("ya empieza la simulación.")

limpiar()
cls_ocultar_cursor()
dibujarTitulo()
print("Presiona 'q' para detenerlo todo en cualquier momento je.")
time.sleep(2)


for fi in range(80):
    for co in range(80):
        if cuadrote[fi][co] == simbolosuelo:
            dibujarPosi(fi + 5, co + 1, simbolosuelo)

while True:
    if msvcrt.kbhit():
        if msvcrt.getch().lower() == b'q':
            break

    otroCua = [[" " for _ in range(80)] for _ in range(80)]

    for i in range(80):
        for j in range(80):
            actual = cuadrote[i][j]
            vecinos = contarCel(cuadrote, i, j)

            if actual == simbolosuelo:
                #se queda igual jeje
                if vecinos == 2 or vecinos == 3:
                    otroCua[i][j] = simbolosuelo
                else: 
                    #muere
                    otroCua[i][j] = " "
            else: 
                #renace
                if vecinos == 3:
                    otroCua[i][j] = simbolosuelo

            if otroCua[i][j] != actual:
                dibujarPosi(i + 5, j + 1, otroCua[i][j])
    
    cuadrote = [fila[:] for fila in otroCua] 
    time.sleep(0.1) # pa la velocidad del juego

limpiar()
cls_mostrar_cursor()