/* Esta es la parte grafica, este es el que debe de imprimir a consola */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Ajedrez.GameObjects
{
    public class Game
    {
        private const int MaxCasillas = 64;
        private const int MaxFichas = 24;
        private const int MaxFilas = 8;
        private const int MaxColumnas = 8;
        private Tablero _tablero;
        private readonly Dictionary<int, string> _diccionarioColumnas;

        public Game()
        {
            _tablero = new Tablero();
            _diccionarioColumnas = _tablero.ColumnaLetraDictionary;
        }

        private string ObtenerNombreCasilla(Casilla casilla)
        {
            var fila = _diccionarioColumnas[casilla.Fila];

            var id = fila + casilla.Columna;

            var nombre = (casilla.Color == ColorCasilla.Negro) ? "N" : "B";

            return "___";
            //return id + nombre;
        }

        private string ObtenerNombreFicha(Pieza pieza)
        {
            var id = pieza.Tipo;
            var diccionario = _tablero.PiezasDictionary;
            var tipo = diccionario[id];
            var prefijo = "";

            switch (tipo)
            {
                case "Peon":
                    prefijo = "P";
                    break;
                case "Torre":
                    prefijo = "R";
                    break;
                case "Caballero":
                    prefijo = "N";
                    break;
                case "Alfil":
                    prefijo = "B";
                    break;
                case "Reina":
                    prefijo = "Q";
                    break;
                case "Rey":
                    prefijo = "K";
                    break;
            }

            var nombre = (pieza.Color == ColorFicha.Negro) ? "N" : "B";

            return "▲" + prefijo + nombre;
        }

        public void DibujarTableroConsola()
        {
            var delimitadorFila = "";
            for (int i = 0; i < (MaxColumnas * 4); i++)
            {
                delimitadorFila += "-";
            }            
            
            for (int fila = MaxFilas; fila >= 1; fila--)
            {
                //var filaName = diccionarioColumnas[fila];
                Console.Write("{0}  ", fila);
                for (int col = 1; col <= MaxColumnas; col++)
                {
                    var casilla = _tablero.GetCasilla(fila, col);
                    Pieza pieza = null;

                    if (casilla.PiezaContenida != null)
                        pieza = casilla.PiezaContenida;

                    var datoImprimir = ObtenerNombreCasilla(casilla);

                    if (pieza != null)
                        datoImprimir = ObtenerNombreFicha(pieza);

                    Console.Write("{0}|", datoImprimir);
                }

                Console.WriteLine("");
                Console.Write("   ");
                Console.WriteLine(delimitadorFila);
            }
            Console.Write("   ");
            Console.Write(" A   B   C   D   E   F   G   H");
            Console.WriteLine("");
        }

        private void ConvertirCasillaAFilaColumna(out int fila, out int columna, string casilla)
        {
            var firstChar = casilla[0].ToString(CultureInfo.InvariantCulture);
            var secondChar = casilla[1].ToString(CultureInfo.InvariantCulture);

            if (int.TryParse(firstChar, out fila))
                columna = _diccionarioColumnas.FirstOrDefault(pair => pair.Value == secondChar.ToUpper()).Key;
            else
            {
                fila = int.Parse(secondChar);
                columna = _diccionarioColumnas.FirstOrDefault(pair => pair.Value == firstChar.ToUpper()).Key;
            }
        }

        private string ConvertirFilaColumnaACasilla(int fila, int columna)
        {
            return _diccionarioColumnas[columna] + fila;
        }

        public void RenderGame()
        {
            int respuesta = 0;
            int filaDestino;
            int columnaDestino;
            int filaOrigen;
            int columnaOrigen;
            Casilla casillaOrigen, casillaDestino;
            string piezaOrigen, piezaDestino;
            IEnumerable<Casilla> posibilitiesList;
            do
            {
                string queDeseaHacer;
                do
                {
                    
                    DibujarTableroConsola();
                    Console.WriteLine("Turno del jugador " + _tablero.CurrentTurn);
                    Console.WriteLine("Seleccione la pieza a mover");
                    piezaOrigen = Console.ReadLine();
                    
                    ConvertirCasillaAFilaColumna(out filaOrigen, out columnaOrigen, piezaOrigen);
                    casillaOrigen = _tablero.GetCasilla(filaOrigen, columnaOrigen);
                    
                    posibilitiesList = _tablero.MovementPosibilitiesList(casillaOrigen);
                    if (posibilitiesList == null)
                    {
                        Console.WriteLine("La Casilla es vacia!");
                        PressAnyKeyToContinue();
                        queDeseaHacer = "";
                        continue;
                    } 
                    Console.WriteLine("Has seleccionado: " + _tablero.PiezasDictionary[casillaOrigen.PiezaContenida.Tipo]);
                    Console.WriteLine("Estos son sus posibles movimientos: ");
                    
                    foreach (var posibleMove in posibilitiesList)
                        Console.Write("♦" + ConvertirFilaColumnaACasilla(posibleMove.Fila, posibleMove.Columna) + " ");
                    
                    Console.WriteLine("\nQue quiere hacer?: ");
                    Console.WriteLine(" 1. Mover la pieza: ");
                    Console.WriteLine(" 2. Seleccionar otra pieza: ");
                    queDeseaHacer = Console.ReadLine();
                } while (queDeseaHacer != "1");

                Console.WriteLine("Seleccione lugar de destino: ");
                if (posibilitiesList == null || !posibilitiesList.Any())
                {
                    Console.WriteLine("No hay movimientos Disponibles");
                    continue;
                }
                piezaDestino = Console.ReadLine();
                ConvertirCasillaAFilaColumna(out filaDestino, out columnaDestino, piezaDestino);
                casillaDestino = _tablero.GetCasilla(filaDestino, columnaDestino);
                if (!posibilitiesList.Contains(casillaDestino))
                {
                    Console.WriteLine("Movimiento invalido");
                    PressAnyKeyToContinue();
                    continue;
                }
                var response = _tablero.MovePiece(casillaOrigen, casillaDestino);
                Console.WriteLine(response + "!");
                if (response != Output.CheckMate) continue;
                Console.WriteLine("El ganador es: " + _tablero.NextTurn());
                PressAnyKeyToContinue();
                respuesta = 3;
            } while (respuesta != 3);
            Console.WriteLine("Gracias por jugar!");
            Console.ReadKey();
        }

        private static void PressAnyKeyToContinue()
        {
            Console.WriteLine("Presiona cualquier tecla para continuar.");
            Console.ReadKey();
        }
    }
}

