using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace not_broforce
{
    class Utils {

        public enum Direction { Up, Down, Left, Right}

        public static Vector3 DirectionToVector3(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    {
                        return Vector3.up;
                    }
                case Direction.Down:
                    {
                        return Vector3.down;
                    }
                case Direction.Left:
                    {
                        return Vector3.left;
                    }
                case Direction.Right:
                    {
                        return Vector3.right;
                    }
                default:
                    {
                        return Vector3.zero;
                    }
            }
        }

        /// <summary>
        /// Calculates the difference between two points' coordinates.
        /// </summary>
        /// <param name="start">a start point</param>
        /// <param name="end">an end point</param>
        /// <returns>the difference between the points' coordinates</returns>
        public static Vector2 Difference(Vector2 start, Vector2 end)
        {
            return new Vector2(end.x - start.x, end.y - start.y);
        }

        /// <summary>
        /// Calculates the absolute difference between two points' x-coordinates.
        /// </summary>
        /// <param name="start">a start point</param>
        /// <param name="end">an end point</param>
        /// <returns>the difference between the points' x-coordinates</returns>
        public static int XDifference(Vector2 start, Vector2 end)
        {
            return (int) (Mathf.Abs(end.x - start.x) + 0.5f);
        }

        /// <summary>
        /// Calculates the absolute difference between two points' y-coordinates.
        /// </summary>
        /// <param name="start">a start point</param>
        /// <param name="end">an end point</param>
        /// <returns>the difference between the points' y-coordinates</returns>
        public static int YDifference(Vector2 start, Vector2 end)
        {
            return (int) (Mathf.Abs(end.y - start.y) + 0.5f);
        }

        /// <summary>
        /// Calculates the distance between two points.
        /// </summary>
        /// <param name="start">a start point</param>
        /// <param name="end">an end point</param>
        /// <returns>the distance between the two points</returns>
        public static float Distance(Vector2 start, Vector2 end)
        {
            return Mathf.Sqrt(Mathf.Pow(end.x - start.x, 2) + Mathf.Pow(end.y - start.y, 2));
        }

        /// <summary>
        /// Calculates the rotation of a line between two points.
        /// </summary>
        /// <param name="start">the start point</param>
        /// <param name="end">the end point</param>
        /// <returns>an angle in radians</returns>
        public static float LineAngle(Vector2 start, Vector2 end)
        {
            if (start.x - end.x < 0)
                return (Mathf.Atan((end.y - start.y) / (end.x - start.x)));       //...
            else if (XDifference(start, end) == 0 && end.y - start.y < 0)
                return -Mathf.PI / 2f;                                            //-90 degrees
            else if (XDifference(start, end) == 0)
                return Mathf.PI / 2f;                                             //90 degrees
            return (Mathf.PI + Mathf.Atan((end.y - start.y) / (end.x - start.x))); //180 degrees + ...
        }

        public static bool ColliderContainsPoint(BoxCollider2D collider,
                                                 Vector3 point)
        {
            return (collider.bounds.Contains(point));

            //Vector3 pointWithSameZ = 
            //    new Vector3(point.x, point.y, collider.transform.position.z);
            //return (collider.bounds.Contains(pointWithSameZ));
        }

        public static bool CollidersIntersect(BoxCollider2D collider1,
                                              BoxCollider2D collider2)
        {
            return (collider1.bounds.Intersects(collider2.bounds));
        }

        public static bool CollidersIntersect(BoxCollider2D collider1,
                                              BoxCollider2D collider2,
                                              float sizeOffset)
        {
            Bounds c1NewBounds = new Bounds(collider1.bounds.center,
                                            collider1.bounds.size * sizeOffset);

            return (c1NewBounds.Intersects(collider2.bounds));
        }

        /// <summary>
        /// Checks if the given character is a number.
        /// </summary>
        /// <param name="c">a character</param>
        /// <returns>is the given character a number</returns>
        public static bool IsNumber(char c)
        {
            if (c >= '0' && c <= '9')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //public static bool CheckIfSameType(Type type1, Type type2)
        //{
        //    return (type1 == type2);
        //}

        //public static bool CheckIfSameTypeOrSubtype(Type subtype, Type type)
        //{
        //    return (subtype.IsSubclassOf(type) ||
        //            CheckIfSameType(subtype, type));
        //}


        public static int WithinLimits(int number, int minLimit, int maxLimit)
        {
            if (minLimit > maxLimit)
            {
                int temp = minLimit;
                minLimit = maxLimit;
                maxLimit = temp;
            }

            if (number < minLimit)
            {
                return minLimit;
            }
            else if (number > maxLimit)
            {
                return maxLimit;
            }
            else
            {
                return number;
            }
        }

        ///// <summary>
        ///// Creates and returns a rectangle based on two elements' positions.
        ///// </summary>
        ///// <param name="topLeftElementPos">the position of an element at
        ///// the top left corner of the area</param>
        ///// <param name="bottomRightElementPos">the position of an element at
        ///// the bottom right corner of the area</param>
        ///// <returns></returns>
        //public static Rectangle ElementArea(Vector2 topLeftElementPos, Vector2 bottomRightElementPos)
        //{
        //    return (new Rectangle((int) topLeftElementPos.X, (int) topLeftElementPos.Y,
        //                          (int) (bottomRightElementPos.X + Level.ElementSideLength - topLeftElementPos.X),
        //                          (int) (bottomRightElementPos.Y + Level.ElementSideLength - topLeftElementPos.Y)));
        //}

        /// <summary>
        /// Calculates the amount of damage dealt.
        /// Damage is not based on attack strength but how many times
        /// can the game character or level element be attacked until it
        /// is destroyed (example: lethal floor kills the character with 2 hits
        /// from full health regardless of the max HP).
        /// </summary>
        /// <param name="maxHitpoints">the victim's maximum hitpoints</param>
        /// <param name="hitsToKill">the attacker's power</param>
        /// <returns>the amount of damage dealt</returns>
        public static int Damage(int maxHitpoints, int hitsToKill)
        {
            if (hitsToKill == 1)
            {
                return maxHitpoints;
            }
            else if (hitsToKill > 1)
            {
                int damage = maxHitpoints / hitsToKill;

                // Integer rounding error correction
                while (damage * hitsToKill < maxHitpoints)
                {
                    damage++;
                }

                return damage;
            }
            else
            {
                return 0;
            }
        }

        //public static void CheckExplosionDamage(PlayerCharacter character, List<Explosion> explosions)
        //{
        //    if (!character.Dead)
        //    {
        //        foreach (Explosion explosion in explosions)
        //        {
        //            if (explosion.lethal &&
        //                (Nav.WithinArea(character.position, explosion.position, explosion.size) ||
        //                 Nav.WithinArea(character.targetPosition, explosion.position, explosion.size)))
        //            {
        //                character.TakeDamage(1);
        //                return;
        //            }
        //        }
        //    }
        //}

        //public static void CheckExplosionDamage(Element element, List<Explosion> explosions)
        //{
        //    if (element.IsFunctional)
        //    {
        //        foreach (Explosion explosion in explosions)
        //        {
        //            if (explosion.lethal &&
        //                (Nav.WithinArea(element.position, explosion.position, explosion.size) ||
        //                 Nav.WithinArea(element.targetPosition, explosion.position, explosion.size)))
        //            {
        //                element.TakeDamage(1, true);
        //            }
        //        }
        //    }
        //}

        public static Vector2 GetScaleFromSize(float actualWidth, float actualHeight, float newWidth, float newHeight)
        {
            int width = (int) (newWidth / actualHeight);
            int height = (int) (newHeight / actualHeight);

            return new Vector2(width, height);
        }

        ///// <summary>
        ///// Splits the given text into lines and returns them.
        ///// </summary>
        ///// <param name="font">a font</param>
        ///// <param name="text">a text</param>
        ///// <param name="maxLength">a line's maximum length</param>
        ///// <returns>lines of text</returns>
        //public static List<string> SplitTextIntoLines(SpriteFont font, string text, float maxLength)
        //{
        //    List<string> lines = new List<string>();

        //    if (text.Length > 0)
        //    {
        //        string line = "";

        //        for (int i = 0; i < text.Length; i++)
        //        {
        //            // If a semicolon is found, a new line is started
        //            if (text[i] == ';')
        //            {
        //                lines.Add(line);

        //                line = "";
        //            }
        //            // If the line has become too long, a new line is started
        //            else if (font.MeasureString(line).X > maxLength)
        //            {
        //                line += text[i];

        //                lines.Add(line);

        //                line = "";
        //            }
        //            // If the text ends, the last line is added to the list
        //            else if (i == text.Length - 1)
        //            {
        //                line += text[i];

        //                lines.Add(line);

        //                line = "";
        //            }
        //            // Otherwise the character is added to the line
        //            else
        //            {
        //                line += text[i];
        //            }
        //        }
        //    }

        //    // Returns the lines
        //    return lines;
        //}

        /// <summary>
        /// Splits the given text into lines and returns them.
        /// Splits the text at each semicolon in it.
        /// </summary>
        /// <param name="text">a text</param>
        /// <returns>lines of text</returns>
        public static List<string> SplitTextIntoLines(string text)
        {
            List<string> lines = new List<string>();

            if (text.Length > 0)
            {
                string line = "";

                for (int i = 0; i < text.Length; i++)
                {
                    // If a semicolon is found, a new line is started
                    if (text[i] == ';')
                    {
                        lines.Add(line);

                        line = "";
                    }
                    // If the text ends, the last line is added to the list
                    else if (i == text.Length - 1)
                    {
                        line += text[i];

                        lines.Add(line);

                        line = "";
                    }
                    // Otherwise the character is added to the line
                    else
                    {
                        line += text[i];
                    }
                }
            }

            // Returns the lines
            return lines;
        }

        public static void MergeStringLists(List<string> lines1, List<string> lines2)
        {
            foreach (string line in lines2)
            {
                lines1.Add(line);
            }
        }

        public static string RomanNumerals(int number)
        {
            // I V X L C D M

            string romanNumerals = "";

            if (number > 1000)
            {
                number = 0;
                romanNumerals = "M+";
            }
            else if (number == 1000)
            {
                number = 0;
                romanNumerals += "M";
            }
            else if (number >= 1000 - 200)
            {
                romanNumerals += RomanNumerals2(number, 1000);
                number -= 800;
                if (number >= 100)
                {
                    number -= 100;
                }
            }
            else if (number >= 500)
            {
                number -= 500;
                romanNumerals += "D";
            }
            else if (number >= 500 - 200)
            {
                romanNumerals += RomanNumerals2(number, 500);
                number -= 300;
                if (number >= 100)
                {
                    number -= 100;
                }
            }
            else if (number >= 100)
            {
                number -= 100;
                romanNumerals += "C";
            }
            else if (number >= 100 - 20)
            {
                romanNumerals += RomanNumerals2(number, 100);
                number -= 80;
                if (number >= 10)
                {
                    number -= 10;
                }
            }
            else if (number >= 50)
            {
                number -= 50;
                romanNumerals += "L";
            }
            else if (number >= 50 - 20)
            {
                romanNumerals += RomanNumerals2(number, 50);
                number -= 30;
                if (number >= 10)
                {
                    number -= 10;
                }
            }
            else if (number >= 10)
            {
                number -= 10;
                romanNumerals += "X";
            }
            else if (number >= 10 - 2)
            {
                romanNumerals += RomanNumerals2(number, 10);
                number -= 8;
                if (number >= 1)
                {
                    number -= 1;
                }
            }
            else if (number >= 5)
            {
                number -= 5;
                romanNumerals += "V";
            }
            else if (number == 4)
            {
                number = 0;
                romanNumerals += "IV";
            }
            else if (number >= 1)
            {
                number -= 1;
                romanNumerals += "I";
            }

            if (number > 0)
            {
                romanNumerals += RomanNumerals(number);
                return romanNumerals;
            }
            else
            {
                return romanNumerals;
            }
        }

        public static string RomanNumerals2(int number, int almostNumber)
        {
            // I V X L C D M

            string romanNumerals = "";

            if (almostNumber == 1000)
            {
                if (number >= 900)
                {
                    romanNumerals += "CM";
                }
                else if (number >= 800)
                {
                    romanNumerals += "DCCC";
                }
            }
            else if (almostNumber == 500)
            {
                if (number >= 400)
                {
                    romanNumerals += "CD";
                }
                else if (number >= 300)
                {
                    romanNumerals += "CCC";
                }
            }
            else if (almostNumber == 100)
            {
                if (number >= 90)
                {
                    romanNumerals += "XC";
                }
                else if (number >= 80)
                {
                    romanNumerals += "LXXX";
                }
            }
            else if (almostNumber == 50)
            {
                if (number >= 40)
                {
                    romanNumerals += "XL";
                }
                else if (number >= 30)
                {
                    romanNumerals += "XXX";
                }
            }
            else if (almostNumber == 10)
            {
                if (number == 9)
                {
                    romanNumerals += "IX";
                }
                else if (number == 8)
                {
                    romanNumerals += "VIII";
                }
            }
            else if (almostNumber == 5)
            {
                if (number == 4)
                {
                    romanNumerals += "IV";
                }
                else if (number == 3)
                {
                    romanNumerals += "III";
                }
            }

            return romanNumerals;
        }
    }
}
