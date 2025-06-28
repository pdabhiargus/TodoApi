# List of Students Example in C#

This example demonstrates how to create a list of objects with three attributes (`name`, `city`, and `rollno`) and print the list.

```csharp
public class Student
{
    public string Name { get; set; }
    public string City { get; set; }
    public int RollNo { get; set; }
}

var students = new List<Student>
{
    new Student { Name = "Alice", City = "Delhi", RollNo = 1 },
    new Student { Name = "Bob", City = "Mumbai", RollNo = 2 },
    new Student { Name = "Charlie", City = "Bangalore", RollNo = 3 }
};

foreach (var student in students)
{
    Console.WriteLine($"Name: {student.Name}, City: {student.City}, RollNo: {student.RollNo}");
}
```

**Output:**
```
Name: Alice, City: Delhi, RollNo: 1
Name: Bob, City: Mumbai, RollNo: 2
Name: Charlie, City: Bangalore, RollNo: 3
```

---

## Multiply RollNo by Itself and Print the List

```csharp
foreach (var student in students)
{
    int squared = student.RollNo * student.RollNo;
    Console.WriteLine($"Name: {student.Name}, City: {student.City}, RollNo: {student.RollNo}, RollNo*RollNo: {squared}");
}
```

**Output:**
```
Name: Alice, City: Delhi, RollNo: 1, RollNo*RollNo: 1
Name: Bob, City: Mumbai, RollNo: 2, RollNo*RollNo: 4
Name: Charlie, City: Bangalore, RollNo: 3, RollNo*RollNo: 9
```

---

## Print 1 2 3 4 5 6 in Pyramid Pattern (C#)

```csharp
int num = 1;
for (int i = 1; i <= 3; i++)
{
    // Print spaces for alignment
    for (int j = 1; j <= 3 - i; j++)
        Console.Write(" ");

    // Print numbers
    for (int k = 1; k <= i; k++)
        Console.Write(num++ + " ");

    Console.WriteLine();
}
```

**Output:**
```
  1 
 2 3 
4 5 6 
```

---

## Filter Out City "Mumbai" and Print the List

```csharp
var filteredStudents = students.Where(s => s.City != "Mumbai").ToList();
foreach (var student in filteredStudents)
{
    Console.WriteLine($"Name: {student.Name}, City: {student.City}, RollNo: {student.RollNo}");
}
```

**Output:**
```
Name: Alice, City: Delhi, RollNo: 1
Name: Charlie, City: Bangalore, RollNo: 3
```

---

## Find All Missing Numbers in a Range (C#)

This version finds and prints all missing numbers in the range, not just one:

```csharp
var numbers = new List<int> { 6, 3, 5, 8 };
int min = numbers.Min();
int max = numbers.Max();

Console.WriteLine($"Min: {min}, Max: {max}");

for (int i = min; i <= max; i++)
{
    if (!numbers.Contains(i))
        Console.WriteLine($"Missing number: {i}");
}
```

**Output:**
```
Min: 3, Max: 8
Missing number: 4
Missing number: 7
```

---

## Time Complexity of an Algorithm

**Time complexity** is a way to describe how the running time of an algorithm increases as the size of the input grows. It helps you estimate the efficiency and scalability of your code.

### Big O Notation
- Time complexity is usually expressed using Big O notation (e.g., O(1), O(n), O(n^2)).
- It describes the upper bound of the running time as the input size (n) increases.

### Common Time Complexities
| Notation   | Name           | Example                        |
|------------|----------------|--------------------------------|
| O(1)       | Constant       | Accessing an array element     |
| O(n)       | Linear         | Loop through a list            |
| O(n^2)     | Quadratic      | Nested loops                   |
| O(log n)   | Logarithmic    | Binary search                  |
| O(n log n) | Linearithmic   | Merge sort, quicksort (avg)    |

### Example: Missing Number Code
```csharp
for (int i = min; i <= max; i++)         // O(n)
{
    if (!numbers.Contains(i))            // O(n) for each Contains
        Console.WriteLine($"Missing number: {i}");
}
```
- Outer loop runs O(n) times (where n = max - min + 1).
- `numbers.Contains(i)` is O(n) for a List.
- Total time complexity: O(n^2) for a List. (Can be improved to O(n) by using a HashSet.)

### Why It Matters
- Lower time complexity means faster code for large inputs.
- Helps you choose the right algorithm and data structure for your problem.

---
