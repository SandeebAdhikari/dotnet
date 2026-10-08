# P0 Banking Application

An individual C# console application that represents a basic banking system for customers and administrators. The application uses object-oriented programming principles, SQL Server for data storage, and Entity Framework Core for database integration.

## Project Information

- **Program:** .NET Fullstack AI Enabled Developer v3 – Cognizant US
- **Project:** P0 – Banking Application
- **Project type:** Individual project
- **Application type:** Console application
- **Start date:** September 3, 2026 at 12:00 AM
- **Due date:** September 23, 2026 at 11:59 PM

## Technology Requirements

- .NET Core
- C#
- SQL Server
- Entity Framework Core
- Object-oriented programming concepts

## Project Objective

The application must provide separate customer and administrator experiences. Users must be able to sign in and access banking operations appropriate to their role. Banking information, account data, credentials, requests, and transaction records must be stored in SQL Server and accessed through Entity Framework Core.

## Main Menu

When the application starts, it must display a welcome message and the following options:

1. Customer
2. Admin
3. Exit

If the user enters any other number, the application must display `Invalid choice` and return to the main menu.

## Customer Requirements

### Customer Login

When `Customer` is selected, the application must request:

- Username
- Password

The initial application skeleton may use predefined credentials. The completed application must validate customer credentials using information stored in the database.

If the credentials are incorrect, the application must:

- Display `Invalid Credential`.
- Return the user to the main menu.

If the credentials are correct, the customer menu must be displayed.

### Customer Menu

An authenticated customer must have access to the following options:

1. Check Account Details
2. Withdraw
3. Deposit
4. Transfer
5. View Last 5 Transactions
6. Request Cheque Book
7. Change Password
8. Exit

### Customer Features

#### Check Account Details

Display the authenticated customer's account information.

#### Withdraw

Allow the customer to withdraw money from an eligible account and record the transaction.

#### Deposit

Allow the customer to deposit money into an account and record the transaction.

#### Transfer

Allow the customer to transfer money to another valid account and record the transaction.

#### Last 5 Transactions

Display the five most recent transactions associated with the customer's account.

#### Request Cheque Book

Allow the customer to submit a cheque-book request for administrator review.

#### Change Password

Allow the authenticated customer to change their password.

#### Exit

End the customer session and leave the customer menu.

For the initial skeleton, each menu choice must display a confirmation message proving that the selected option is recognized. For example, the transfer option may display a transfer-successful message, and the password option may display a password-change message.

## Administrator Requirements

### Administrator Login

When `Admin` is selected, the application must request:

- Username
- Password

The initial application skeleton may use predefined credentials. The completed application must validate administrator credentials using information stored in the database.

If the credentials are incorrect, the application must:

- Display `Invalid Credential`.
- Return the user to the main menu.

If the credentials are correct, the administrator menu must be displayed.

### Administrator Menu

An authenticated administrator must have access to the following options:

1. Create New Account
2. Delete Account
3. Edit Account Details
4. Display Summary
5. Reset Customer Password
6. Approve Cheque Book Request
7. Exit

### Administrator Features

#### Create New Account

Create and store a new customer banking account.

#### Delete Account

Remove an eligible banking account from the system.

#### Edit Account Details

Update the stored information for an existing account.

#### Display Summary

Display a summary of relevant banking or account information.

#### Reset Customer Password

Reset the password associated with a customer account.

#### Approve Cheque Book Request

Review and approve a cheque-book request submitted by a customer.

#### Exit

End the administrator session and leave the administrator menu.

## Application Behavior Requirements

The application must:

- Use repeating menus so that the application continues until an appropriate `Exit` option is selected.
- Process menu choices using selection logic.
- Display clear messages for valid and invalid actions.
- Handle invalid menu selections.
- Handle invalid or unexpected user input.
- Use exception handling to prevent unhandled application failures.
- Use meaningful names for variables, methods, classes, and other program elements.
- Separate customer operations from administrator operations.
- Apply object-oriented programming concepts throughout the application.
- Store and retrieve persistent banking data using SQL Server.
- Integrate the application with the database using Entity Framework Core.

## Functional Checklist

### General

- [ ] Welcome screen is displayed when the application starts.
- [ ] Main menu contains Customer, Admin, and Exit options.
- [ ] Invalid main-menu choices display an error message.
- [ ] Menus repeat until the relevant Exit option is selected.
- [ ] Invalid and unexpected input is handled.
- [ ] Exceptions are handled without unexpectedly terminating the application.
- [ ] Variables, methods, and classes use clear and meaningful names.
- [ ] Object-oriented programming principles are applied.

### Customer

- [ ] Customer login is available.
- [ ] Customer credentials are validated.
- [ ] Invalid credentials display an error and return to the main menu.
- [ ] Account details can be viewed.
- [ ] Withdrawals can be completed and recorded.
- [ ] Deposits can be completed and recorded.
- [ ] Transfers can be completed and recorded.
- [ ] The customer's last five transactions can be viewed.
- [ ] A cheque-book request can be submitted.
- [ ] The customer can change their password.
- [ ] The customer can exit their session.

### Administrator

- [ ] Administrator login is available.
- [ ] Administrator credentials are validated.
- [ ] Invalid credentials display an error and return to the main menu.
- [ ] New accounts can be created.
- [ ] Existing accounts can be deleted.
- [ ] Existing account details can be edited.
- [ ] A banking or account summary can be displayed.
- [ ] Customer passwords can be reset.
- [ ] Cheque-book requests can be approved.
- [ ] The administrator can exit their session.

### Database

- [ ] SQL Server is configured as the application database.
- [ ] Entity Framework Core is integrated with the application.
- [ ] Customer and administrator credentials are stored and validated through the database.
- [ ] Account information is stored persistently.
- [ ] Transaction history is stored persistently.
- [ ] Cheque-book requests and their status are stored persistently.

## Completion Criteria

The project is complete when all required customer and administrator operations are available, invalid input and application errors are handled, banking data is persisted in SQL Server through Entity Framework Core, and the functional checklist has been satisfied.
