﻿using Spectre.Console;
using cacheMe512.Flashcards.Models;
using cacheMe512.Flashcards.Controllers;

namespace cacheMe512.Flashcards.UI
{
    internal class FlashcardUI
    {
        private readonly Stack _stack;
        private static readonly FlashcardController _flashcardController = new();

        public FlashcardUI(Stack stack)
        {
            _stack = stack;
        }

        public void Show()
        {
            while (true)
            {
                Console.Clear();
                AnsiConsole.MarkupLine($"[bold yellow]=== Managing Flashcards for Stack: {_stack.Name} ===[/]");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold cyan]Select an option[/]")
                        .PageSize(4)
                        .AddChoices(new[]
                        {
                            "View Flashcards",
                            "Add Flashcard",
                            "Delete Flashcard",
                            "Back to Stacks"
                        })
                );

                if (choice == "Back to Stacks")
                    return;

                HandleOption(choice);
            }
        }

        private void HandleOption(string option)
        {
            switch (option)
            {
                case "View Flashcards":
                    ViewFlashcards();
                    break;
                case "Add Flashcard":
                    AddFlashcard();
                    break;
                case "Delete Flashcard":
                    DeleteFlashcard();
                    break;
            }
        }

        private void ViewFlashcards()
        {
            var flashcards = _flashcardController.GetFlashcardsByStackId(_stack.Id);

            if (!flashcards.Any())
            {
                Utilities.DisplayMessage("No flashcards available in this stack.", "red");
                AnsiConsole.MarkupLine("Press Any Key to Continue.");
                Console.ReadKey();
                return;
            }

            var table = new Table();
            table.AddColumn("[yellow]ID[/]");
            table.AddColumn("[yellow]Question[/]");
            table.AddColumn("[yellow]Answer[/]");

            foreach (var flashcard in flashcards)
            {
                table.AddRow(
                    flashcard.Id.ToString(),
                    $"[cyan]{flashcard.Question}[/]",
                    $"[green]{flashcard.Answer}[/]"
                );
            }

            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine("Press Any Key to Continue.");
            Console.ReadKey();
        }

        private void AddFlashcard()
        {
            string question = AnsiConsole.Ask<string>("Enter the flashcard question:");
            string answer = AnsiConsole.Ask<string>("Enter the flashcard answer:");

            var newFlashcard = new Flashcard
            {
                StackId = _stack.Id,
                Question = question,
                Answer = answer
            };

            _flashcardController.InsertFlashcard(newFlashcard);
            Utilities.DisplayMessage("Flashcard added successfully!", "green");
            AnsiConsole.MarkupLine("Press Any Key to Continue.");
            Console.ReadKey();
        }

        private void DeleteFlashcard()
        {
            var flashcards = _flashcardController.GetFlashcardsByStackId(_stack.Id);

            if (!flashcards.Any())
            {
                Utilities.DisplayMessage("No flashcards available to delete.", "red");
                AnsiConsole.MarkupLine("Press Any Key to Continue.");
                Console.ReadKey();
                return;
            }

            var selectedFlashcard = AnsiConsole.Prompt(
                new SelectionPrompt<Flashcard>()
                    .Title("[bold yellow]Select a flashcard to delete[/]")
                    .PageSize(10)
                    .UseConverter(fc => $"{fc.Id}: {fc.Question}")
                    .AddChoices(flashcards)
            );

            _flashcardController.DeleteFlashcard(selectedFlashcard.Id);
            Utilities.DisplayMessage("Flashcard deleted successfully!", "red");
            AnsiConsole.MarkupLine("Press Any Key to Continue.");
            Console.ReadKey();
        }
    }
}
