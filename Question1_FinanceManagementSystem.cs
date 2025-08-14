
using System;
using System.Collections.Generic;

namespace FinanceManagement
{
    // Q1a: Define core models using records
    public readonly record struct Transaction(int Id, DateTime Date, decimal Amount, string Category);

    // Q1b: Implement payment behavior using interfaces
    public interface ITransactionProcessor
    {
        void Process(Transaction transaction);
    }

    // Q1c: Concrete processors
    public class BankTransferProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[BankTransfer] Processing {transaction.Category} of {transaction.Amount:C} on {transaction.Date:yyyy-MM-dd} (Tx #{transaction.Id})");
        }
    }

    public class MobileMoneyProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[MobileMoney] Processing {transaction.Category} of {transaction.Amount:C} on {transaction.Date:yyyy-MM-dd} (Tx #{transaction.Id})");
        }
    }

    public class CryptoWalletProcessor : ITransactionProcessor
    {
        public void Process(Transaction transaction)
        {
            Console.WriteLine($"[CryptoWallet] Processing {transaction.Category} of {transaction.Amount:C} on {transaction.Date:yyyy-MM-dd} (Tx #{transaction.Id})");
        }
    }

    // Q1d: Base Account
    public class Account
    {
        public string AccountNumber { get; init; }
        public decimal Balance { get; protected set; }

        public Account(string accountNumber, decimal initialBalance)
        {
            if (string.IsNullOrWhiteSpace(accountNumber))
                throw new ArgumentException("Account number is required.", nameof(accountNumber));
            if (initialBalance < 0)
                throw new ArgumentOutOfRangeException(nameof(initialBalance), "Initial balance cannot be negative.");
            
            AccountNumber = accountNumber;
            Balance = initialBalance;
        }

        // virtual: default behavior deducts amount
        public virtual void ApplyTransaction(Transaction transaction)
        {
            Balance -= transaction.Amount;
            Console.WriteLine($"[Account] Applied {transaction.Category} of {transaction.Amount:C}. New balance: {Balance:C}");
        }
    }

    // Q1e: Sealed SavingsAccount
    public sealed class SavingsAccount : Account
    {
        public SavingsAccount(string accountNumber, decimal initialBalance) 
            : base(accountNumber, initialBalance) { }

        public override void ApplyTransaction(Transaction transaction)
        {
            if (transaction.Amount > Balance)
            {
                Console.WriteLine("Insufficient funds");
                return;
            }

            Balance -= transaction.Amount;
            Console.WriteLine($"[SavingsAccount] Deducted {transaction.Amount:C} for {transaction.Category}. Updated balance: {Balance:C}");
        }
    }

    // Q1f: FinanceApp orchestrator
    public class FinanceApp
    {
        private readonly List<Transaction> _transactions = new();

        public void Run()
        {
            Console.WriteLine("=== Finance Management System (Q1) ===");

            // i. Instantiate a SavingsAccount
            var account = new SavingsAccount(accountNumber: "ACC-001", initialBalance: 1000m);
            Console.WriteLine($"Created SavingsAccount #{account.AccountNumber} with initial balance {account.Balance:C}");

            // ii. Create three Transaction records
            var t1 = new Transaction(Id: 1, Date: DateTime.Now, Amount: 120.50m, Category: "Groceries");
            var t2 = new Transaction(Id: 2, Date: DateTime.Now, Amount: 300.00m, Category: "Utilities");
            var t3 = new Transaction(Id: 3, Date: DateTime.Now, Amount: 700.00m, Category: "Entertainment");

            // iii. Process with respective processors
            ITransactionProcessor mobileMoney = new MobileMoneyProcessor();
            ITransactionProcessor bankTransfer = new BankTransferProcessor();
            ITransactionProcessor cryptoWallet = new CryptoWalletProcessor();

            mobileMoney.Process(t1);     // Transaction 1
            bankTransfer.Process(t2);    // Transaction 2
            cryptoWallet.Process(t3);    // Transaction 3

            // iv. Apply each transaction to SavingsAccount
            account.ApplyTransaction(t1);
            account.ApplyTransaction(t2);
            account.ApplyTransaction(t3); // This one may hit "Insufficient funds" depending on remaining balance

            // v. Add all transactions to _transactions
            _transactions.AddRange(new[] { t1, t2, t3 });

            Console.WriteLine("\n--- Transactions Logged ---");
            foreach (var tx in _transactions)
            {
                Console.WriteLine($"Tx #{tx.Id}: {tx.Category} - {tx.Amount:C} on {tx.Date:yyyy-MM-dd}");
            }

            Console.WriteLine($"\nFinal Balance for {account.AccountNumber}: {account.Balance:C}");
            Console.WriteLine("=== End ===");
        }
    }

    // Main entry
    public static class Program
    {
        public static void Main()
        {
            var app = new FinanceApp();
            app.Run();
        }
    }
}
