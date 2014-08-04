﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Ajedrez.GameObjects
{
    using PieceInit = Tuple<int, int, ColorFicha, TipoPieza>;
    public enum Output
    {
        Success,Check,OutOfBounds,InvalidMove,SelfCheck,CancelledMove
    }

    enum Direccion
    {
        Up,Down,Left,Right,RightUp,LeftUp,RightDown,LeftDown    
    }
    enum TipoPieza
    {
        Peon,Torre,Caballero,Alfil,Reina,Rey
    }

    public enum ColorJugador
    {
        Blanco,Negro
    }
        public class Tablero
        {
            private List<PieceInit> _initialValueSets =
                new List<PieceInit>();
            
            private readonly Dictionary<int, string> _columnaLetraDictionary = new Dictionary<int, string>
            {
                {1,"A"},{2,"B"},{3,"C"},{4,"D"},{5,"E"},{6,"F"},{7,"G"},{8,"H"}
            };

            private readonly Dictionary<int, string> _piezasDictionary = new Dictionary<int, string>
            {
                {0, "Peon"},{1, "Torre"},{2, "Caballero"},{3, "Alfil"},{4, "Reina"},{5, "Rey"}
            };
            public Dictionary<int, string> ColumnaLetraDictionary{get { return _columnaLetraDictionary; }}
            public Dictionary<int, string> PiezasDictionary{get { return _piezasDictionary; }}
            
            private const int MaxCasillas = 64;
            private const int MaxFichas = 24;
            private const int MaxFilas = 8;
            private const int MaxColumnas = 8;
            private int _piezaContador;

            
            private List<Pieza> _piezas;
            private List<Casilla> _casillas;

            public Tablero()
            {
                _piezas = new List<Pieza>(MaxFichas);
                _casillas = new List<Casilla>(MaxCasillas);
                CurrentTurn = ColorJugador.Blanco;                
                InicializarPosiciones();
                CrearCasillas();
                LlenarCasillas();
            }

            private void InicializarPosiciones()
            {
                for (var i = 1; i <= 8; i++)
                    _initialValueSets.Add(new PieceInit(2, i, ColorFicha.Blanco, TipoPieza.Peon));
                for (var i = 1; i <= 8; i++)
                    _initialValueSets.Add(new PieceInit(7, i, ColorFicha.Negro, TipoPieza.Peon));
                _initialValueSets.Add(new PieceInit(1, 1, ColorFicha.Blanco, TipoPieza.Torre));                
                _initialValueSets.Add(new PieceInit(1, 2, ColorFicha.Blanco, TipoPieza.Caballero));
                _initialValueSets.Add(new PieceInit(1, 3, ColorFicha.Blanco, TipoPieza.Alfil));
                _initialValueSets.Add(new PieceInit(1, 4, ColorFicha.Blanco, TipoPieza.Reina));
                _initialValueSets.Add(new PieceInit(1, 5, ColorFicha.Blanco, TipoPieza.Rey));
                _initialValueSets.Add(new PieceInit(1, 6, ColorFicha.Blanco, TipoPieza.Alfil));
                _initialValueSets.Add(new PieceInit(1, 7, ColorFicha.Blanco, TipoPieza.Caballero));
                _initialValueSets.Add(new PieceInit(1, 8, ColorFicha.Blanco, TipoPieza.Torre));

                _initialValueSets.Add(new PieceInit(8, 1, ColorFicha.Negro, TipoPieza.Torre));
                _initialValueSets.Add(new PieceInit(8, 2, ColorFicha.Negro, TipoPieza.Caballero));
                _initialValueSets.Add(new PieceInit(8, 3, ColorFicha.Negro, TipoPieza.Alfil));
                _initialValueSets.Add(new PieceInit(8, 4, ColorFicha.Negro, TipoPieza.Reina));
                _initialValueSets.Add(new PieceInit(8, 5, ColorFicha.Negro, TipoPieza.Rey));
                _initialValueSets.Add(new PieceInit(8, 6, ColorFicha.Negro, TipoPieza.Alfil));
                _initialValueSets.Add(new PieceInit(8, 7, ColorFicha.Negro, TipoPieza.Caballero));
                _initialValueSets.Add(new PieceInit(8, 8, ColorFicha.Negro, TipoPieza.Torre));
            }

            private void LlenarCasillas()
            {
                foreach (var initValSet in _initialValueSets)
                {
                    GetCasilla(initValSet.Item1, initValSet.Item2).PiezaContenida = 
                        CrearPieza(initValSet.Item4,initValSet.Item3);
                }
            }

            private  Pieza CrearPieza(TipoPieza tipo,ColorFicha color)
            {   var newPiece =  new Pieza
                {
                    Color = color,
                    Id = _piezaContador++,
                    Tipo = (int)tipo
                };
                _piezas.Add(newPiece);
                return newPiece;
            }

            private void CrearCasillas()
            {
                var tempColor = ColorCasilla.Negro;
                var idtemp = 0;

                for (var columna = 1; columna <= MaxColumnas; columna++)
                {
                    for (var fila = 1; fila <= MaxFilas; fila++)
                    {
                        _casillas.Add
                            (
                            new Casilla
                                {
                                    Color = tempColor,
                                    Columna = columna,
                                    Fila = fila,
                                    Id = idtemp++
                                }
                            );
                        tempColor = tempColor == ColorCasilla.Negro ? ColorCasilla.Blanco : ColorCasilla.Negro;
                    }
                }
            }
            public Casilla GetCasilla(int fila, int columna)
            {
                var casillas = _casillas.AsEnumerable().Where(casilla => casilla.Columna == columna && casilla.Fila == fila).ToArray();
                return casillas.Any() ? casillas[0] : null;
            }
            public Casilla GetCasilla(int id)
            {
                return _casillas.Find(casilla => casilla.Id == id);
            }

            private Pieza GetPiezaDeCasilla(int idCasilla)
            {
                var pieza = _casillas.First(casilla => casilla.PiezaContenida != null && casilla.Id == idCasilla).PiezaContenida;
                return pieza;
                //SI una casilla tiene mas de 1 ficha, hay un ERROR
            }
            private Pieza GetPiezaDeCasilla(int fila, int columna)
            {
                return GetPiezaDeCasilla(GetCasilla(fila,columna).Id);
            }
            public Output MoverPieza(int idCasillaOrigen, int idCasillaDestuno)
            {
                return Output.Success; //TODO
            }
            public Output MoverPieza(int filaOrigen, int columnaOrigen, int destinoFila, int destinoColumna)
            {
                return Output.Success;//TODO
            }
          public bool DeadLock()
            {
             //Verificar cada pieza para ver si esta locked. //TODO
                return false;
            }

            public List<Casilla> MovementPosibilitiesList(Casilla casilla)
            {
                var piezaEnCasilla = casilla.PiezaContenida;
                return piezaEnCasilla == null ? null : MovimientoPieza(casilla);
            }

            private List<Casilla> MovimientoPieza(Casilla casilla)
            {
                var king = GetKing(casilla.PiezaContenida.Color);
                var returnList = new List<Casilla>();
                switch ((TipoPieza)casilla.PiezaContenida.Tipo)
                {
                    case TipoPieza.Peon:
                        returnList.AddRange(PeonRangeCheck(casilla));
                        break;
                    case TipoPieza.Torre:
                        returnList.AddRange(RangeCheck(casilla,casilla,Direccion.Up));
                        returnList.AddRange(RangeCheck(casilla,casilla,Direccion.Down));
                        returnList.AddRange(RangeCheck(casilla,casilla,Direccion.Left));
                        returnList.AddRange(RangeCheck(casilla,casilla,Direccion.Right));
                        break;
                    case TipoPieza.Caballero:
                        returnList.AddRange(CaballeroRangeCheck(casilla));
                        break;
                    case TipoPieza.Alfil:
                        returnList.AddRange(RangeCheck(casilla, casilla, Direccion.LeftUp));
                        returnList.AddRange(RangeCheck(casilla, casilla, Direccion.LeftDown));
                        returnList.AddRange(RangeCheck(casilla, casilla, Direccion.RightDown));
                        returnList.AddRange(RangeCheck(casilla, casilla, Direccion.RightUp));
                        break;
                    case TipoPieza.Reina:
                        returnList.AddRange(RangeCheck(casilla, casilla, Direccion.Up));
                        returnList.AddRange(RangeCheck(casilla, casilla, Direccion.RightUp));
                        returnList.AddRange(RangeCheck(casilla, casilla, Direccion.Right));
                        returnList.AddRange(RangeCheck(casilla, casilla, Direccion.RightDown));
                        returnList.AddRange(RangeCheck(casilla, casilla, Direccion.Down));
                        returnList.AddRange(RangeCheck(casilla, casilla, Direccion.LeftDown));
                        returnList.AddRange(RangeCheck(casilla, casilla, Direccion.Left));
                        returnList.AddRange(RangeCheck(casilla, casilla, Direccion.LeftUp));
                        break;
                    case TipoPieza.Rey:
                        returnList.AddRange(RangeCheck(casilla, casilla, Direccion.Up,1));
                        returnList.AddRange(RangeCheck(casilla, casilla, Direccion.RightUp,1));
                        returnList.AddRange(RangeCheck(casilla, casilla, Direccion.Right,1));
                        returnList.AddRange(RangeCheck(casilla, casilla, Direccion.RightDown,1));
                        returnList.AddRange(RangeCheck(casilla, casilla, Direccion.Down,1));
                        returnList.AddRange(RangeCheck(casilla, casilla, Direccion.LeftDown,1));
                        returnList.AddRange(RangeCheck(casilla, casilla, Direccion.Left,1));
                        returnList.AddRange(RangeCheck(casilla, casilla, Direccion.LeftUp,1));
                        returnList.RemoveAll(casilla1 => CasillasEnPeligro(casilla).Contains(casilla1));
                        break;
                }
                return returnList;
            }

            public Output checkMate()
            {
                // Revisar todos los movimientos posibles de las piezas del color que esta en jaque
                // Si no existe ningun movimiento legal, declarar jaque mate (que no regrese selfcheck mover)
                // Tambien revisar cada turno cuando ya no hayan movimientos legales que un jugador pueda hacer,
                //pero no esta en Jaque en el momemnto declarar StaleMate
                return Output.Success;
            }
            private IEnumerable<Casilla> CaballeroRangeCheck(Casilla casilla)
            {
                var returnList = new List<Casilla>();
                for (var i = 2; i >= -2; i--)
                {
                    if(i==0) continue;
                    var j = (i == 2|| i ==-2) ? 1 : 2;
                    returnList.AddRange(_casillas.Where(casilla1 => casilla1.Fila == casilla.Fila + i
                        && casilla1.Columna == casilla.Columna + j
                        && (casilla1.PiezaContenida == null || casilla1.PiezaContenida.Color != casilla.PiezaContenida.Color)));
                    returnList.AddRange(_casillas.Where(casilla1 => casilla1.Fila == casilla.Fila + i
                        && casilla1.Columna == casilla.Columna - j
                        && (casilla1.PiezaContenida == null || casilla1.PiezaContenida.Color != casilla.PiezaContenida.Color)));
                }
                return returnList;
            }

            public Output MovePiece(Casilla casillaOrigen,Casilla casillaDestino)
            {
                var piezaOrigen = casillaOrigen.PiezaContenida;
                var piezaDesino = casillaDestino.PiezaContenida;
                    casillaDestino.PiezaContenida = casillaOrigen.PiezaContenida;
                    casillaOrigen.PiezaContenida = null;

                if (CheckCheck(piezaOrigen.Color))
                {
                    casillaOrigen.PiezaContenida = piezaOrigen;
                    casillaDestino.PiezaContenida = piezaDesino;
                    return Output.SelfCheck;
                }
                if (CheckCheck(piezaOrigen.Color == ColorFicha.Blanco ? ColorFicha.Negro : ColorFicha.Blanco))
                {
                    NextTurn();
                    return Output.Check;
                }
                NextTurn();
                    return Output.Success;
            }

            private void NextTurn()
            {
                CurrentTurn = CurrentTurn == ColorJugador.Blanco ? ColorJugador.Negro : ColorJugador.Blanco;
            }

            public ColorJugador CurrentTurn { get; set; }

            public bool CheckCheck(ColorFicha color)
            {
                var king = GetKing(color);
                var enPeligro = CasillasEnPeligro(king);
                var isKingChecked = enPeligro.Contains(king);
                return isKingChecked;
            }

            private Casilla GetKing(ColorFicha color)
            {      
                return _casillas.FirstOrDefault(casilla => casilla.PiezaContenida != null 
                                                        && casilla.PiezaContenida.Tipo == (int) TipoPieza.Rey
                                                        && casilla.PiezaContenida.Color == color);
            }

            private List<Casilla> CasillasEnPeligro(Casilla king)
            {
                var colorContrario = king.PiezaContenida.Color == ColorFicha.Blanco ? ColorFicha.Negro : ColorFicha.Blanco;
                var enemyPieces = _casillas.FindAll(casilla => casilla.PiezaContenida.Color == colorContrario);
                var casillasEnPeligro = new List<Casilla>();
                foreach (var enemyPiece in enemyPieces)
                {
                    casillasEnPeligro.AddRange(MovementPosibilitiesList(enemyPiece));
                }
                return casillasEnPeligro;
            }

            private IEnumerable<Casilla> PeonRangeCheck(Casilla casilla)
            {
                Direccion direccion;
                int range;
                var returnValue = new List<Casilla>();
                switch (casilla.PiezaContenida.Color)
                {
                    case ColorFicha.Blanco:
                        range = casilla.Fila == 2 ? 2 : 1;
                        direccion = Direccion.Up;
                        break;
                    case ColorFicha.Negro:
                        range = casilla.Fila == 7 ? 2 : 1;
                        direccion = Direccion.Down;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                returnValue.AddRange(RangeCheck(casilla,casilla,direccion,range));
                returnValue.AddRange(PeonCaptureCheck(casilla));
                return returnValue;
            }

            private IEnumerable<Casilla> PeonCaptureCheck(Casilla casilla)
            {
                var returnList = new List<Casilla>();
                if (casilla.PiezaContenida.Color == ColorFicha.Blanco)
                {
                    var enemigoUR = GetCasilla(casilla.Fila + 1, casilla.Columna + 1);
                    var enemigoUL = GetCasilla(casilla.Fila + 1, casilla.Columna - 1);
                    if (enemigoUL.PiezaContenida != null) returnList.Add(enemigoUL);
                    if (enemigoUR.PiezaContenida != null) returnList.Add(enemigoUR);
                }
                else
                {
                    var enemigoDR = GetCasilla(casilla.Fila - 1, casilla.Columna + 1);
                    var enemigoDL = GetCasilla(casilla.Fila - 1, casilla.Columna - 1);
                    if (enemigoDL.PiezaContenida != null) returnList.Add(enemigoDL);
                    if (enemigoDR.PiezaContenida != null) returnList.Add(enemigoDR);
                }
                return returnList;
            }
            private IEnumerable<Casilla> RangeCheck(Casilla casillaOrigen,Casilla nextCasilla, Direccion direccion,int maximumRange = 9)
            {
                var returnList = new List<Casilla>();
                if (maximumRange == 0) return returnList;
                nextCasilla = NextCasilla(casillaOrigen, nextCasilla,direccion);
                if (nextCasilla == null) return returnList;
                returnList.Add(nextCasilla);
                maximumRange--;
                if(nextCasilla.PiezaContenida == null)
                    returnList.AddRange(RangeCheck(casillaOrigen,nextCasilla,direccion,maximumRange));
                return returnList;
            }

            private Casilla NextCasilla(Casilla casillaOrigen, Casilla nextCasilla, Direccion direccion)
            {
                var filaOffset = FilaOffset(direccion);
                var columnaOffset = ColumnaOffset(direccion);

                return _casillas.FirstOrDefault(casilla1 => casilla1.Fila == nextCasilla.Fila + filaOffset 
                    && casilla1.Columna == nextCasilla.Columna + columnaOffset
                    && (casilla1.PiezaContenida == null 
                        || (casilla1.PiezaContenida.Color != casillaOrigen.PiezaContenida.Color
                        && (TipoPieza)casillaOrigen.PiezaContenida.Tipo != TipoPieza.Peon)
                        ));
            }

            private static int FilaOffset(Direccion direccion)
            {
                var filaOffset = 0;
                switch (direccion)
                {
                    case Direccion.LeftUp:
                    case Direccion.RightUp:
                    case Direccion.Up:
                        filaOffset = 1;
                        break;
                    case Direccion.RightDown:
                    case Direccion.LeftDown:
                    case Direccion.Down:
                        filaOffset = -1;
                        break;
                }
                return filaOffset;
            }

            private static int ColumnaOffset(Direccion direccion)
            {
                var columnaOffset = 0;
                switch (direccion)
                {
                    case Direccion.LeftDown:
                    case Direccion.LeftUp:
                    case Direccion.Left:
                        columnaOffset = 1;
                        break;
                    case Direccion.RightUp:
                    case Direccion.RightDown:
                    case Direccion.Right:
                        columnaOffset = -1;
                        break;
                }
                return columnaOffset;
            }


            private bool DeadLock(Pieza pieza)
            {
                //ver cuantas opciones retorna 
                return false;
            }

            public List<Casilla> SelectPiece(int fila,int columna)
            {
                return MovementPosibilitiesList(GetCasilla(fila, columna));
            }
        }
    }
