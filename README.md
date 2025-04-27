# 📦 AddressPicker

**Address Autocomplete .NET Control using [Geoapify](https://www.geoapify.com/)**

A simple, customizable WPF control for address autocomplete in .NET applications.

---

![AddressPicker Demo](https://github.com/user-attachments/assets/5378a7ab-88fb-4c90-845f-755ce14a7234)

---

# 🚀 NuGet Package

You can install AddressPicker directly from NuGet:

🔗 [View on NuGet.org](https://www.nuget.org/packages/AddressPicker)

```bash
Install-Package AddressPicker
```

# 🛠 Code Sample
Here's a basic example of how to use the AddressPicker control inside a WPF Window:

```xaml
<Window x:Class="Sandbox.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:bluebyte="http://schemas.bluebyte.biz/wpf.controls"
        mc:Ignorable="d"
        Title="MainWindow" Width="800">
    
    <Window.Resources>
        <BooleanToVisibilityConverter x:Key="Visibility" />
    </Window.Resources>

    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition />
            <RowDefinition />
        </Grid.RowDefinitions>

        <StackPanel Orientation="Vertical" Margin="5">
            <TextBlock Text="Address Picker (by Blue Byte Systems Inc.):" />
            <bluebyte:AddressPicker Name="AddressPicker" APIKey="YOUR-GEOAPIFY-API-KEY" />
        </StackPanel>

        <StackPanel Grid.Row="2" Orientation="Vertical" Margin="5">
            <TextBlock Text="Street:" />
            <TextBox Margin="5" Text="{Binding SelectedAddress.AddressLine1, ElementName=AddressPicker}" />
            <TextBox Margin="5" Text="{Binding SelectedAddress.AddressLine2, ElementName=AddressPicker}" />

            <TextBlock Text="City:" />
            <TextBox Margin="5" Text="{Binding SelectedAddress.City, ElementName=AddressPicker}" />

            <TextBlock Text="State (Code):" />
            <TextBox Margin="5" Text="{Binding SelectedAddress.StateCode, ElementName=AddressPicker}" />

            <TextBlock Text="Zip Code:" />
            <TextBox Margin="5" Text="{Binding SelectedAddress.Postcode, ElementName=AddressPicker}" />

            <TextBlock Text="Country:" />
            <TextBox Margin="5" Text="{Binding SelectedAddress.Country, ElementName=AddressPicker}" />
        </StackPanel>
    </Grid>
</Window>
```
# 📋 Features

- 🔥 Easy integration into WPF applications
- 🔎 Real-time address suggestions powered by Geoapify
- 🎯 Simple two-way binding to selected address fields
- 🛡️ API key protected
- 🌍 Supports multiple address components (Street, City, State, Zip Code, Country)


# 📜 License
AddressPicker is licensed under the MIT License.

✨ About
Created and maintained by (Blue Byte Systems Inc.)[https://bluebyte.biz]
Helping .NET developers integrate address autocomplete seamlessly in their apps!
