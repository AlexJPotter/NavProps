# NavProps

Source-generated helpers to simplify working with navigation properties in Entity Framework.

> Please note this project is currently in alpha and is not strictly ready for production use just yet. Please raise an issue on GitHub if you encounter any problems or would like to give feedback.

## Motivation

Entity Framework navigation properties are a great way to simplify working with related entities. However, they can be a bit of a pain to work with when it comes to nullability.

[Current guidance](https://learn.microsoft.com/en-us/ef/core/performance/efficient-querying#beware-of-lazy-loading) discourages the use of lazy loading, with eager loading being the preferred approach. This means that navigation properties will be `null` at runtime if they have not been loaded, and will throw an opaque exception if you attempt to access them. It also means either littering your code with non-null assertions (using the `!` operator), or introducing lots of boilerplate code to check for nulls.

Furthermore, the nullability of navigation properties is not always obvious. In relationships where the foreign key is nullable, the navigation property may be `null` at runtime even if it has been loaded. In these cases you need to check both the foreign key and the navigation property to determine whether the value should be `null`, or if the navigation property has not been loaded.

After working with Entity Framework on a number of large codebases and getting quite frustrated with the amount of boilerplate code I was having to write, I decided to see if I could come up with a better solution. This is the result.

## Features

- Source-generated `NavProps()` function for each Entity Framework model class
- Automatic null-checks on navigation properties returned through `NavProps()`
- Intelligent nullability based on the nullability of the foreign key property
- Exception messages that provide precise information about the navigation property that was not loaded

## Installation

Install the `NavProps` package from NuGet and ensure the `OutputItemType` and `ReferenceOutputAssembly` properties are set as follows in your `.csproj` file:

```xml
<PackageReference
    Include="NavProps"
    Version="0.0.1-alpha"
    OutputItemType="Analyzer"
    ReferenceOutputAssembly="false"
/>
```

## Usage

Every class (`T`) referenced in a `DbSet<T>` property within a Data Context class (i.e. one that derives from `DbContext`) will have a `NavProps()` function generated for it.

The `NavProps()` function will return a class consisting of properties that wrap the navigation properties defined on the class. Each wrapped property will have the correct nullability based on the optional nature of the navigation property (as defined by the nullability of the foreign key property).

Attempting to access a navigation property via the class returned by `NavProps()` will throw an exception if the navigation property has not been loaded. The exception message contains information about both the navigation property and the class it is defined on.

### Example

Consider the following class:

```csharp
public class Order
{
    public int OrderId { get; private set; }

    public int CustomerId { get; private set; }
    public Customer? Customer { get; private set; }

    public int? PromotionId { get; private set; }
    public Promotion? Promotion { get; private set; }

    public ICollection<OrderItem>? OrderItems { get; private set; }
}
```

Each Order has exactly one Customer it is associated with - this is indicated by the fact that the `CustomerId` property is a non-nullable `int`. The `Customer` property is an EF navigation property, and therefore is of type `Customer?` to indicate that it could be `null` if it has not been loaded.

An Order may be associated with a particular Promotion, but this is optional - this is indicated by the fact that the `PromotionId` property is of type `int?`. The `Promotion` property is an EF navigation property, and therefore is of type `Promotion?` to indicate that it could be `null` if it has not been loaded.

An Order may have zero or more Order Items associated with it. The `OrderItems` property is an EF navigation property, and therefore is of type `ICollection<OrderItem>?` to indicate that it could be `null` if it has not been loaded.

The following code shows how the `NavProps()` function can be used to access the navigation properties of an Order:

```csharp
Order order = ...; // Imagine the order has been obtained somehow

// This will throw an exception if the Customer navigation property has not been loaded.
// Note that the type here is `Customer` and not `Customer?`, since the FK is non-nullable,
// meaning the navigation property will never be null if it has been loaded.
Customer customer = order.NavProps().Customer;
Console.WriteLine($"Customer: {customer.Name}");

// This will throw an exception if the `PromotionId` property is non-null but the Promotion
// is null, since the presence of a non-null FK means the navigation property will not be
// null if it has been loaded. This will return `null` if the `PromotionId` property is null.
Promotion? promotion = order.NavProps().Promotion;
Console.WriteLine($"Promotion: {promotion?.Name ?? "-"}");

// This will throw an exception if the `OrderItems` navigation property has not been loaded.
// Note that the type here is `ICollection<OrderItem>` and not `ICollection<OrderItem>?`,
// since the property should always be non-null if it has been loaded, even if it is just
// an empty collection.
ICollection<OrderItem> orderItems = order.NavProps().OrderItems;
Console.WriteLine($"Order Items: {orderItems.Count}");
```

If we were to attempt to access the `Promotion` navigation property when the `PromotionId` property is non-null but the `Promotion` navigation property is `null`, we would get the following exception:

```
Navigation property 'Promotion' on class 'Order' was not loaded
```
