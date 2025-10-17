using System;
using System.Collections.Generic;

class Account
{
    public string AccountNumber { get; set; }
    public string AccountName { get; set; }
    public string UserName { get; set; }
    public string AccountType { get; set; }
    public double Balance { get; set; } = 0;
    public string Pin { get; private set; }
    public string SecurityQuestion { get; private set; }
    public string SecurityAnswer { get; private set; }

    public Account(string accountNumber, string accountName, string userName, string accountType, string pin, string securityQuestion, string securityAnswer)
    {
        AccountNumber = accountNumber;
        AccountName = accountName;
        UserName = userName;
        AccountType = accountType;
        Pin = pin;
        SecurityQuestion = securityQuestion;
        SecurityAnswer = securityAnswer.ToLower();
    }

    public bool VerifyPin()
    {
        Console.Write("Enter your 4-digit PIN: ");
        string enteredPin = Program.ReadHiddenInput();

        if (enteredPin == Pin)
            return true;

        Console.WriteLine("\n❌ Incorrect PIN!");
        return false;
    }

    public void Deposit(double amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("❌ Deposit amount must be greater than zero!");
            return;
        }

        Balance += amount;
        Console.WriteLine($"✓ Successfully deposited ₦{amount}. New balance: ₦{Balance}");
    }

    public void Withdraw(double amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("❌ Withdrawal amount must be greater than zero!");
            return;
        }

        if (amount > Balance)
        {
            Console.WriteLine($"❌ Insufficient funds! Your balance is ₦{Balance}");
            return;
        }

        Balance -= amount;
        Console.WriteLine($"✓ Successfully withdrew ₦{amount}. New balance: ₦{Balance}");
    }

    public void CheckBalance()
    {
        Console.WriteLine($"\nAccount: {AccountNumber} | Name: {AccountName} | Type: {AccountType} | Balance: ₦{Balance}");
    }

    public void TransferTo(Account receiverAccount, double amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("❌ Transfer amount must be greater than zero!");
            return;
        }

        if (amount > Balance)
        {
            Console.WriteLine($"❌ Insufficient funds! Your balance is ₦{Balance}");
            return;
        }

        Balance -= amount;
        receiverAccount.Balance += amount;

        Console.WriteLine($"✓ Transferred ₦{amount} to {receiverAccount.AccountName} ({receiverAccount.AccountNumber})");
        Console.WriteLine($"New balance: ₦{Balance}");
    }

    public void InterbankTransfer(string receivingBank, string receiverAccountNumber, string receiverName, double amount)
    {
        if (amount <= 0)
        {
            Console.WriteLine("❌ Transfer amount must be greater than zero!");
            return;
        }

        if (amount > Balance)
        {
            Console.WriteLine($"❌ Insufficient funds! Your balance is ₦{Balance}");
            return;
        }

        Balance -= amount;
        Console.WriteLine($"✓ Interbank transfer of ₦{amount} sent to {receiverName} ({receiverAccountNumber}) at {receivingBank}.");
        Console.WriteLine($"Your new balance: ₦{Balance}");
    }

    public void ChangePin()
    {
        Console.Write("Enter your current PIN: ");
        string oldPin = Program.ReadHiddenInput();

        if (oldPin != Pin)
        {
            Console.WriteLine("\n❌ Incorrect current PIN!");
            return;
        }

        Console.Write("Enter new 4-digit PIN: ");
        string newPin = Program.ReadHiddenInput();

        if (newPin.Length != 4)
        {
            Console.WriteLine("\n❌ PIN must be 4 digits!");
            return;
        }

        Console.Write("Confirm new PIN: ");
        string confirmPin = Program.ReadHiddenInput();

        if (newPin != confirmPin)
        {
            Console.WriteLine("\n❌ PINs do not match!");
            return;
        }

        Pin = newPin;
        Console.WriteLine("\n✓ PIN changed successfully!");
    }

    public bool ResetPin()
    {
        Console.WriteLine($"\nSecurity Question: {SecurityQuestion}");
        Console.Write("Answer: ");
        string answer = Console.ReadLine().ToLower();

        if (answer == SecurityAnswer)
        {
            Console.Write("Enter new 4-digit PIN: ");
            string newPin = Program.ReadHiddenInput();

            if (newPin.Length != 4)
            {
                Console.WriteLine("\n❌ PIN must be 4 digits!");
                return false;
            }

            Pin = newPin;
            Console.WriteLine("\n✓ PIN reset successfully!");
            return true;
        }
        else
        {
            Console.WriteLine("❌ Incorrect security answer!");
            return false;
        }
    }
}

class Bank
{
    private List<Account> accounts = new List<Account>();

    public void CreateAccount(string accountNumber, string accountName, string userName, string accountType, string pin, string question, string answer)
    {
        foreach (Account acc in accounts)
        {
            if (acc.AccountNumber == accountNumber)
            {
                Console.WriteLine("❌ Account already exists!");
                return;
            }
        }

        Account newAccount = new Account(accountNumber, accountName, userName, accountType, pin, question, answer);
        accounts.Add(newAccount);
        Console.WriteLine($"\n✓ {accountType} account created successfully for {accountName}!");
    }

    public Account FindAccount(string accountNumber)
    {
        foreach (Account acc in accounts)
        {
            if (acc.AccountNumber == accountNumber)
                return acc;
        }
        return null;
    }

    public Account Login(string accountNumber, string pin)
    {
        Account account = FindAccount(accountNumber);
        if (account != null && account.Pin == pin)
        {
            Console.WriteLine($"\n✓ Welcome back, {account.UserName}!");
            return account;
        }

        Console.WriteLine("❌ Invalid account number or PIN!");
        return null;
    }

    public bool ResetPin(string accountNumber)
    {
        Account account = FindAccount(accountNumber);
        if (account == null)
        {
            Console.WriteLine("❌ Account not found!");
            return false;
        }

        return account.ResetPin();
    }
}

class Program
{
    static void Main()
    {
        Bank myBank = new Bank();
        Account loggedInAccount = null;

        while (true)
        {
            if (loggedInAccount == null)
            {
                Console.WriteLine("\n= Moshood BANK =");
                Console.WriteLine("1. Create Account");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Reset PIN");
                Console.WriteLine("4. Exit");
                Console.Write("Choose an option (1-4): ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateNewAccount(myBank);
                        break;
                    case "2":
                        loggedInAccount = Login(myBank);
                        break;
                    case "3":
                        ResetAccountPin(myBank);
                        break;
                    case "4":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("❌ Invalid choice!");
                        break;
                }
            }
            else
            {
                Console.WriteLine($"\n= Welcome {loggedInAccount.UserName}! =");
                Console.WriteLine("1. Deposit");
                Console.WriteLine("2. Withdraw");
                Console.WriteLine("3. Check Balance");
                Console.WriteLine("4. Transfer (Same Bank)");
                Console.WriteLine("5. Interbank Transfer");
                Console.WriteLine("6. Change PIN");
                Console.WriteLine("7. Logout");
                Console.Write("Choose an option (1-7): ");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        DepositMoney(loggedInAccount);
                        break;
                    case "2":
                        WithdrawMoney(loggedInAccount);
                        break;
                    case "3":
                        loggedInAccount.CheckBalance();
                        break;
                    case "4":
                        TransferMoney(myBank, loggedInAccount);
                        break;
                    case "5":
                        InterbankTransfer(loggedInAccount);
                        break;
                    case "6":
                        loggedInAccount.ChangePin();
                        break;
                    case "7":
                        loggedInAccount = null;
                        Console.WriteLine("✓ Logged out successfully.");
                        break;
                    default:
                        Console.WriteLine("❌ Invalid choice!");
                        break;
                }
            }
        }
    }

    static void CreateNewAccount(Bank bank)
    {
        Console.Write("Enter account number: ");
        string accountNumber = Console.ReadLine();

        Console.Write("Enter account name: ");
        string accountName = Console.ReadLine();

        Console.Write("Enter username: ");
        string userName = Console.ReadLine();

        Console.Write("Enter account type (Savings/Current): ");
        string accountType = Console.ReadLine();

        if (accountType.ToLower() != "savings" && accountType.ToLower() != "current")
        {
            Console.WriteLine("❌ Invalid account type! Must be 'Savings' or 'Current'.");
            return;
        }

        Console.Write("Enter 4-digit PIN: ");
        string pin = ReadHiddenInput();

        if (pin.Length != 4)
        {
            Console.WriteLine("\n❌ PIN must be exactly 4 digits!");
            return;
        }

        Console.Write("\nSet a security question (e.g., What is your pet’s name?): ");
        string question = Console.ReadLine();

        Console.Write("Answer: ");
        string answer = Console.ReadLine();

        bank.CreateAccount(accountNumber, accountName, userName, accountType, pin, question, answer);
    }

    static Account Login(Bank bank)
    {
        Console.Write("Enter account number: ");
        string accountNumber = Console.ReadLine();

        Console.Write("Enter PIN: ");
        string pin = ReadHiddenInput();

        return bank.Login(accountNumber, pin);
    }

    static void DepositMoney(Account account)
    {
        Console.Write("Enter deposit amount: ");
        if (double.TryParse(Console.ReadLine(), out double amount))
            account.Deposit(amount);
        else
            Console.WriteLine("❌ Invalid amount entered!");
    }

    static void WithdrawMoney(Account account)
    {
        Console.Write("Enter withdrawal amount: ");
        if (double.TryParse(Console.ReadLine(), out double amount))
            account.Withdraw(amount);
        else
            Console.WriteLine("❌ Invalid amount entered!");
    }

    static void TransferMoney(Bank bank, Account senderAccount)
    {
        Console.Write("Enter receiver's account number: ");
        string receiverAccountNumber = Console.ReadLine();

        Account receiverAccount = bank.FindAccount(receiverAccountNumber);
        if (receiverAccount == null)
        {
            Console.WriteLine("❌ Receiver account not found!");
            return;
        }

        Console.Write("Enter transfer amount: ");
        if (double.TryParse(Console.ReadLine(), out double amount))
            senderAccount.TransferTo(receiverAccount, amount);
        else
            Console.WriteLine("❌ Invalid amount entered!");
    }

    static void InterbankTransfer(Account senderAccount)
    {
        Console.Write("Enter receiving bank name: ");
        string receivingBank = Console.ReadLine();

        Console.Write("Enter receiver's account number: ");
        string receiverAccountNumber = Console.ReadLine();

        Console.Write("Enter receiver's name: ");
        string receiverName = Console.ReadLine();

        Console.Write("Enter transfer amount: ");
        if (double.TryParse(Console.ReadLine(), out double amount))
            senderAccount.InterbankTransfer(receivingBank, receiverAccountNumber, receiverName, amount);
        else
            Console.WriteLine("❌ Invalid amount entered!");
    }

    static void ResetAccountPin(Bank bank)
    {
        Console.Write("Enter your account number: ");
        string accountNumber = Console.ReadLine();
        bank.ResetPin(accountNumber);
    }

    public static string ReadHiddenInput()
    {
        string input = "";
        ConsoleKey key;
        do
        {
            var keyInfo = Console.ReadKey(intercept: true);
            key = keyInfo.Key;

            if (key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input.Substring(0, input.Length - 1);
                Console.Write("\b \b");
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                input += keyInfo.KeyChar;
                Console.Write("*");
            }
        } while (key != ConsoleKey.Enter);
        Console.WriteLine();
        return input;
    }
}
