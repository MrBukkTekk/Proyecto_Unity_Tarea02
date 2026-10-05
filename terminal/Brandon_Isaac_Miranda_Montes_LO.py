# El de ayuda muestra al usuario que botones pulsar para acabar el juego, no las instrucciones y es con una lista 
# que vaya registrando las luces, todo va centrado (todo de todo), ponerle el murcielago, mencionar las columnas (pa que el profe sepa)
# bajar la figura para que no coma el titulo

import random
import os
import comandos_consola as consola
import msvcrt 

colorLuz   = "6A079C" 
colorApa   = "C54071"
simboSuelo = "█" 
simboLuz   = "■" 
simboJuga  = "°"

MURCIELAGO_BASE = [
    "      XXXXXX                                XXXXXX               ",
    "   XXXXXX   X           X        X            XX  XXXXXX           ",
    "XXXX       XX         XXXX    XXXX           X        XXXX        ",
    "XXX            XX        XX XXXXXXX XX         XX           XXXX     ",
    "XX                XXXX     X           X     XXXXX               XXX   ",
    "X                    XXXXXXXX         XXXXXXXX                     XX  ",
    "X                                                                    XX ",
    "X      XXXXXX                                               XXXXXX      XX",
    "X    XXXXXX    XXXX                                       XXXX    XXXXX  XX",
    "X  XXX           XX                                     XX            XXXXX",
    "XXX               XXXXXXXXX                     XXXXXXXXX               XXX",
    "                          XXXX             XXXX        X                  ",
    "                             XXXX         XXXX                            ",
    "                               XXXX     XXXX                              ",
    "                                  XX   XX                                 ",
    "                                   X                                      "
]

def tituloChilo():
    consola.limpiar_pantalla()
    print(r"""                ('-. .-. .-') _     .-')                                     .-') _    
               ( OO )  /(  OO) )   ( OO ).                                  (  OO) )   
        ,--.    ,-.-')   ,----.    ,--. ,--./     '._ (_)---\_)       .-'),-----.  ,--. ,--.  /     '._  
        |  |.-')  |  |OO) '  .-./-') |  | |  ||'--...__)/     _ |       ( OO'  .-.  ' |  | |  |  |'--...__) 
        |  | OO ) |  |  \ |  |_( O- )|  . |  |'--.  .--'\  :` `.       /   |  | |  | |  | | .-')'--.  .--' 
        |  |`-' | |  |(_/ |  | .--, \|      |   |  |    '..`''.)      \_) |  |\|  | |  |_|( OO )  |  |    
       (|  '---.',|  |_.'(|  | '. (_/|  .-.  |   |  |   .-._)   \        \ |  | |  | |  | | `-' /  |  |    
        |      |(_|  |    |  '--'  | |  | |  |   |  |   \       /         `'  '-'  '('  '-'(_.-'   |  |    
        `------'  `--'     `------'  `--' `--'   `--'    `-----'            `-----'   `-----'      `--'    
          Hecho por Brandon Isaac Miranda Montes jeje... """)
    print("-" * 110)

def leer_tecla():
    char = msvcrt.getch()
    if char == b'\x00' or char == b'\xe0':
        codigo = msvcrt.getch()
        if codigo == b'H': 
            return "up"
        if codigo == b'P': 
            return "down"
        if codigo == b'K': 
            return "left"
        if codigo == b'M': 
            return "right"
    if char == b'\r': 
        return "enter"
    if char == b'\x1b': 
        return "esc"
    try: 
        return char.decode('utf-8').lower()
    except: return ""

def bati_Fondo():
    consola.establecer_color_hex("444444") 
    for i, linea in enumerate(MURCIELAGO_BASE):
        consola.mover_cursor(18 + i, 23) 
        consola.imprimir(linea)
    consola.restaurar_colores()

def dibujarCeldita(f, c, estado, tam):
    centrof = 26 - (tam / 2)
    centroc = 59 - int(tam * 1.5)
    consola.mover_cursor(int(centrof + f), int(centroc + (c * 3)))

    if estado == 1:
        consola.establecer_color_hex(colorLuz) 
        consola.imprimir(simboLuz)
    else:
        consola.establecer_color_hex(colorApa) 
        consola.imprimir(simboSuelo)
    consola.restaurar_colores()

def ayudaPasitos(solucion):
    consola.limpiar_pantalla()
    print("=== MODO AYUDA: REGISTRO DE LUCES TOCADAS ===")

    for i, m in enumerate(solucion):
        print("Pista ", i + 1, ": Fila ", m[0] + 1, " Columna ", m[1] + 1)
    print("\nPresiona ENTER para volver...")

    while leer_tecla() != "enter": 
        pass

def iniciar_partida(tamCuadro, nombre_difi):
    tablerolo = [[0 for _ in range(tamCuadro)] for _ in range(tamCuadro)]
    puntosValidos = [(f, c) for f in range(tamCuadro) for c in range(tamCuadro)]
    num_clics = 4 if tamCuadro == 5 else (6 if tamCuadro == 7 else 8)
    solucion = random.sample(puntosValidos, num_clics)

    for sf, sc in solucion:
        for f, c in [(0,0), (1,0), (-1,0), (0,1), (0,-1)]:
            nf, nc = sf + f, sc + c
            if 0 <= nf < tamCuadro and 0 <= nc < tamCuadro:
                tablerolo[nf][nc] = 1 - tablerolo[nf][nc]

    fila_actual, col_actual = 0, 0
    tituloChilo()
    print(f" NIVEL: {nombre_difi} | FLECHAS: Mover | ENTER: Luz | H: Ayuda | Q: Salir")
    print("-" * 110)
    bati_Fondo()
    for f in range(tamCuadro):
        for c in range(tamCuadro):
            dibujarCeldita(f, c, tablerolo[f][c], tamCuadro)
            
    while True:
        centro_f = 26 - (tamCuadro / 2)
        centro_c = 59 - int(tamCuadro * 1.5)
        consola.mover_cursor(int(centro_f + fila_actual), int(centro_c + (col_actual * 3)))
        consola.imprimir(simboJuga)
        tecla = leer_tecla()
        if tecla in ("q", "esc"): 
            break
        if tecla == "h":
            ayudaPasitos(solucion)
            tituloChilo(); print(" NIVEL:" , nombre_difi , "| FLECHAS: Mover | ENTER: Luz | H: Ayuda | Q: Salir"); print("-" * 110)
            bati_Fondo()
            for f in range(tamCuadro):
                for c in range(tamCuadro): 
                    dibujarCeldita(f, c, tablerolo[f][c], tamCuadro)
            continue

        dibujarCeldita(fila_actual, col_actual, tablerolo[fila_actual][col_actual], tamCuadro)
        if tecla == "up" and fila_actual > 0: 
            fila_actual -= 1
        elif tecla == "down" and fila_actual < tamCuadro - 1: 
            fila_actual += 1
        elif tecla == "left" and col_actual > 0: 
            col_actual -= 1
        elif tecla == "right" and col_actual < tamCuadro - 1: 
            col_actual += 1
        elif tecla == "enter":
            for df, dc in [(0,0), (1,0), (-1,0), (0,1), (0,-1)]:
                nf, nc = fila_actual + df, col_actual + dc
                if 0 <= nf < tamCuadro and 0 <= nc < tamCuadro:
                    tablerolo[nf][nc] = 1 - tablerolo[nf][nc]
                    dibujarCeldita(nf, nc, tablerolo[nf][nc], tamCuadro)
            if sum(sum(r) for r in tablerolo) == 0:
                consola.mover_cursor(15, 40); consola.establecer_color_hex("00FF00"); print(" GANASTE! [ENTER] PA SALIR :DDD "); consola.restaurar_colores()
                while leer_tecla() != "enter": 
                    pass
                break

def menu_principal():
    os.system("mode con: cols=125 lines=55") 
    consola.ocultar_cursor()
    opcion = 1
    while True:
        tituloChilo() 
        p = ["   "] * 4; p[opcion-1] = ">> "
        print("\n" + " " * 42 + p[0] + "1. Facil   (5x5)")
        print(" " * 42 + p[1] + "2. Normal  (7x7)")
        print(" " * 42 + p[2] + "3. MUERTE  (9x9)")
        print(" " * 42 + p[3] + "Q. Salir")
        tecla = leer_tecla()
        if tecla in ("q", "esc"): break
        elif tecla == "up": 
            opcion = 4 if opcion == 1 else opcion - 1
        elif tecla == "down": 
            opcion = 1 if opcion == 4 else opcion + 1
        elif tecla == "enter":
            if opcion == 1: 
                iniciar_partida(5, "FACIL")
            elif opcion == 2:
                iniciar_partida(7, "NORMAL")
            elif opcion == 3: 
                iniciar_partida(9, "MUERTE")
            elif opcion == 4: 
                break 

if __name__ == "__main__":
    menu_principal()
