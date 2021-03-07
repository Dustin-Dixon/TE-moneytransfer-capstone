using System;
using System.Collections.Generic;
using TenmoClient.Data;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();
        private static readonly UserService userService = new UserService();
        private static readonly BankingService bankingService = new BankingService();

        static void Main(string[] args)
        {
            authService.RegisterLoginHandler(userService);
            authService.RegisterLoginHandler(bankingService);
            Run();
        }

        private static void Run()
        {
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!userService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = consoleService.PromptForLogin();
                        authService.Login(loginUser);
                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered = false;
                    while (!isRegistered) //will keep looping until user is registered
                    {
                        LoginUser registerUser = consoleService.PromptForLogin();
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                            loginRegister = -1; //reset outer loop to allow choice for login
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }

            MenuSelection();
        }

        private static void MenuSelection()
        {
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == 1)
                {
                    API_Account currentAccount = bankingService.GetCurrentAccount();
                    if (currentAccount != null)
                    {
                        Console.WriteLine($"Your current account balance is: {currentAccount.Balance:C2}");
                    }
                }
                else if (menuSelection == 2)
                {
                    // call ViewTransfers server side
                    List<API_Transfer> transfers = bankingService.GetTransfers();
                    if (transfers != null)
                    {
                        consoleService.DisplayTransfers(transfers, userService.GetUserId());
                        int transferId = consoleService.PromptForTransferID("view details");
                        if (transferId != 0)
                        {
                            API_Transfer transferToView = bankingService.GetTransferById(transferId);
                            if (transferToView != null)
                            {
                                consoleService.DisplayTransferDetails(transferToView);
                            }
                        }
                    }
                }
                else if (menuSelection == 3)
                {
                    // TODO: link up client here
                }
                else if (menuSelection == 4)
                {
                    List<UserInfo> users = bankingService.GetAllUsers();

                    if (users != null)
                    {
                        List<UserInfo> displayUsers = new List<UserInfo>();
                        foreach (UserInfo user in users)
                        {
                            if (user.UserId != userService.GetUserId())
                            {
                                displayUsers.Add(user);
                            }
                        }

                        consoleService.DisplayUsers(displayUsers);
                        int toUserId = consoleService.PromptForUserID("sending to");
                        if (toUserId != 0)
                        {
                            decimal amountToSend = consoleService.PromptForAmount();

                            UserInfo fromUser = new UserInfo()
                            {
                                UserId = userService.GetUserId()
                            };
                            UserInfo toUser = new UserInfo()
                            {
                                UserId = toUserId
                            };
                            API_Transfer transfer = new API_Transfer
                            {
                                FromUser = fromUser,
                                ToUser = toUser,
                                Amount = amountToSend
                            };

                            bankingService.SendTransfer(transfer, "send");
                        }
                    }
                }
                else if (menuSelection == 5)
                {
                    List<UserInfo> users = bankingService.GetAllUsers();

                    if (users != null)
                    {
                        List<UserInfo> displayUsers = new List<UserInfo>();
                        foreach (UserInfo user in users)
                        {
                            if (user.UserId != userService.GetUserId())
                            {
                                displayUsers.Add(user);
                            }
                        }

                        consoleService.DisplayUsers(displayUsers);
                        int fromUserId = consoleService.PromptForUserID("requesting from");
                        if (fromUserId != 0)
                        {
                            decimal amountToSend = consoleService.PromptForAmount();

                            UserInfo fromUser = new UserInfo()
                            {
                                UserId = fromUserId
                            };
                            UserInfo toUser = new UserInfo()
                            {
                                UserId = userService.GetUserId()
                            };
                            API_Transfer transfer = new API_Transfer
                            {
                                FromUser = fromUser,
                                ToUser = toUser,
                                Amount = amountToSend
                            };

                            bankingService.SendTransfer(transfer, "request");
                        }
                    }
                }
                else if (menuSelection == 6)
                {
                    Console.WriteLine("");
                    authService.Logout(); //wipe out previous login info
                    Run(); //return to entry point
                }
                else
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }
    }
}
