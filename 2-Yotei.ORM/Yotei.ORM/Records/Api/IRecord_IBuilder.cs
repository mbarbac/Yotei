namespace Yotei.ORM;

partial interface IRecord
{
    // ====================================================
    /// <summary>
    /// Represents a builder of <see cref="IRecord"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : IEnumerable<object?>
    {
        /// <summary>
        /// Returns a immutable record based upon the contents of this instance.
        /// </summary>
        /// <returns></returns>
        IRecord ToInstance();

        // ------------------------------------------------

        /// <summary>
        /// Gets the number of elements in this instance.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets or sets the value at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        object? this[int index] { get; set; }

        /// <summary>
        /// Determines if this instance carries any element that matches the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Contains(Predicate<object?> predicate);

        /// <summary>
        /// Returns the index of the first element that matches the given predicate, or -1 if any.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int IndexOf(Predicate<object?> predicate);

        /// <summary>
        /// Returns the index of the last element that matches the given predicate, or -1 if any.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int LastIndexOf(Predicate<object?> predicate);

        /// <summary>
        /// Returns the indexes of all the elements that match the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        List<int> IndexesOf(Predicate<object?> predicate);

        /// <summary>
        /// Gets a list with the values in this instance.
        /// </summary>
        /// <returns></returns>
        List<object?> ToList();

        /// <summary>
        /// Gets a list with the given number of values from this instance, starting at the given
        /// index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        List<object?> ToList(int index, int count);

        /// <summary>
        /// Gets an array with the values in this instance.
        /// </summary>
        /// <returns></returns>
        object?[] ToArray();

        // ------------------------------------------------

        /// <summary>
        /// Replaces the value at the given index with the new given one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Replace(int index, object? value);

        /// <summary>
        /// Adds the given value to this instance.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Add(object? value);

        /// <summary>
        /// Adds the values of the given range to this instance.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<object?> range);

        /// <summary>
        /// Inserts the given value into this instance at the given index.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Insert(int index, object? value);

        /// <summary>
        /// Inserts the values of the given range into this instance, starting at the given index.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        bool InsertRange(int index, IEnumerable<object?> range);

        /// <summary>
        /// Removes the value at the given index, shrinking the size of this instance.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool RemoveAt(int index);

        /// <summary>
        /// Removes the given number of values starting at the given index, shrinking the size of
        /// this instance.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        bool RemoveRange(int index, int count);

        /// <summary>
        /// Removes from this instance the first element that matches the given predicate, and if
        /// so, shrinks the size of this instance.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Remove(Predicate<object?> predicate);

        /// <summary>
        /// Removes from this instance the last element that matches the given predicate, and if
        /// so, shrinks the size of this instance.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool RemoveLast(Predicate<object?> predicate);

        /// <summary>
        /// Removes from this instance all the elements that match the given predicate, and if so,
        /// shrinks the size of this instance.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool RemoveAll(Predicate<object?> predicate);

        /// <summary>
        /// Clears this instance and resets its size to zero.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}