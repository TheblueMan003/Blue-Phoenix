using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System;
using System.Drawing;
using System.ComponentModel;

namespace JSharp
{
    /// <summary>
    /// Tag a colorer: mot clé, nombre, chaine de caractères...
    /// </summary>
    struct Tag
    {
        /// <summary>
        /// Constructeur du Tag. Permet de définir ces paramètres
        /// </summary>
        /// <param name="start">Index de départ du tag</param>
        /// <param name="len">Nombres de caractères du tag</param>
        /// <param name="color">Couleur à appliquer</param>
        /// <param name="style">Style de police à appliquer</param>
        public Tag(int start, int len, Color color, FontStyle style)
        {
            iStart = start;
            iLength = len;
            cColor = color;
            fStyle = style;
        }

        /// <summary>
        /// Index de départ du tag
        /// </summary>
        public int iStart;
        /// <summary>
        /// Nombres de caractères du tag
        /// </summary>
        public int iLength;
        /// <summary>
        /// Couleur à appliquer
        /// </summary>
        public Color cColor;
        /// <summary>
        /// Style de police à appliquer
        /// </summary>
        public FontStyle fStyle;
    }

    /// <summary>
    /// Structure de définition groupes d'éléments à rechercher
    /// </summary>
    struct Coloring
    {
        /// <summary>
        /// Expression régulière permettant de trouver les éléments
        /// </summary>
        public Regex regex;
        /// <summary>
        /// Couleur à appliquer pour cette recherche
        /// </summary>
        public Color cColor;
        /// <summary>
        /// Style à appliquer pour cette recherche
        /// </summary>
        public FontStyle fStyle;
    }

    /// <summary>
    /// Classe principale de l'objet. Doit remplacer les RichTextBox!
    /// 
    /// Cette classe a une gestion automatique d'indentation et de coloration syntaxique.
    /// </summary>
    public class ColorTextBox : RichTextBox
    {

        public Control hideSelection = new Control();
    }
}