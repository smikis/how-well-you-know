using FluentAssertions;
using KnowMe.API.Domain.Entities;

namespace KnowMe.API.Domain.Tests;

public class QuestionTests
{
    [Fact]
    public void Create_WhenValidParametersGiven_CreatesQuestion()
    {
        var user = User.Create("player1").Value;
        var gameResult = Game.Create("test game", user).Value;

        var questionVariants = new Dictionary<char, string>()
        {
            { 'A', "Yes" },
            { 'B', "No" }
        };

        var questionResult = Question.Create("Is this the real life?", false, questionVariants, user, gameResult);
        questionResult.IsSuccess.Should().BeTrue();
        questionResult.Value.Text.Should().Be("Is this the real life?");
    }

    [Fact]
    public void Create_WhenTooLongQuestionTextGiven_ReturnsError()
    {
        var user = User.Create("player1").Value;
        var gameResult = Game.Create("test game", user).Value;

        var questionVariants = new Dictionary<char, string>()
        {
            { 'A', "Yes" },
            { 'B', "No" }
        };

        var questionResult = Question.Create("Is this the real life?Is this the real life?Is this the real life?Is this the real life?Is this the real life?", false, questionVariants, user, gameResult);
        questionResult.IsSuccess.Should().BeFalse();
        questionResult.Errors!.First().Message.Should().Be("Question text cannot be longer than 100 characters");
    }

    [Fact]
    public void Create_WhenTooLongQuestionVariantTextGiven_ReturnsError()
    {
        var user = User.Create("player1").Value;
        var gameResult = Game.Create("test game", user).Value;

        var questionVariants = new Dictionary<char, string>
        {
            { 'A', "YesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYesYes" },
            { 'B', "No" }
        };

        var questionResult = Question.Create("Is this the real life?", false, questionVariants, user, gameResult);
        questionResult.IsSuccess.Should().BeFalse();
        questionResult.Errors!.First().Message.Should().Be("Answer text cannot be longer than 100 characters");
    }

    [Fact]
    public void Create_WhenNotEnoughQuestionVariantsGiven_ReturnsError()
    {
        var user = User.Create("player1").Value;
        var gameResult = Game.Create("test game", user).Value;

        var questionVariants = new Dictionary<char, string>()
        {
            { 'A', "Yes" }
        };

        var questionResult = Question.Create("Is this the real life?", false, questionVariants, user, gameResult);
        questionResult.IsSuccess.Should().BeFalse();
        questionResult.Errors!.First().Message.Should().Be("More than one possible question answer must be added");
    }

    [Fact]
    public void Create_WhenTooMuchQuestionVariantsGiven_ReturnsError()
    {
        var user = User.Create("player1").Value;
        var gameResult = Game.Create("test game", user).Value;

        var questionVariants = new Dictionary<char, string>()
        {
            { 'A', "Yes" },
            { 'B', "Yes" },
            { 'C', "Yes" },
            { 'D', "Yes" },
            { 'E', "Yes" },
            { 'F', "Yes" },
            { 'G', "Yes" },
            { 'H', "Yes" },
            { 'I', "Yes" },
            { 'J', "Yes" },
            { 'K', "Yes" },
            { 'L', "Yes" },
            { 'M', "Yes" },
            { 'N', "Yes" },
            { 'O', "Yes" },
            { 'P', "Yes" },
            { 'Q', "Yes" },
            { 'R', "Yes" },
            { 'S', "Yes" },
            { 'T', "Yes" },
            { 'X', "Yes" },
            { 'Y', "Yes" }
        };

        var questionResult = Question.Create("Is this the real life?", false, questionVariants, user, gameResult);
        questionResult.IsSuccess.Should().BeFalse();
        questionResult.Errors!.First().Message.Should().Be("No more than twenty possible question answer must be added");
    }

    [Fact]
    public void RecordChoice_GivenUserChoice_RecordsIt()
    {
        var user = User.Create("player1").Value;
        var gameResult = Game.Create("test game", user).Value;

        var questionVariants = new Dictionary<char, string>()
        {
            { 'A', "Yes" },
            { 'B', "No" }
        };

        var question = Question.Create("Is this the real life?", false, questionVariants, user, gameResult).Value;

        var userChoice = QuestionUserChoice.Create(user, question, [question.AnswerVariants.First(v => v.Notation == 'A')]);

        var result = question.RecordChoice(userChoice.Value);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void RecordChoice_GivenDuplicateUserChoice_ReturnsError()
    {
        var user = User.Create("player1").Value;
        var gameResult = Game.Create("test game", user).Value;

        var questionVariants = new Dictionary<char, string>()
        {
            { 'A', "Yes" },
            { 'B', "No" }
        };

        var question = Question.Create("Is this the real life?", false, questionVariants, user, gameResult).Value;

        var userChoice = QuestionUserChoice.Create(user, question, [question.AnswerVariants.First(v => v.Notation == 'A')]);

        question.RecordChoice(userChoice.Value);

        var result = question.RecordChoice(userChoice.Value);
        result.IsSuccess.Should().BeFalse();
        result.Errors!.First().Message.Should().Be("User already made choice");
    }

    [Fact]
    public void RecordGuess_GivenUserGuess_RecordsIt()
    {
        var user = User.Create("player1").Value;
        var user2 = User.Create("player2").Value;
        var gameResult = Game.Create("test game", user).Value;

        var questionVariants = new Dictionary<char, string>()
        {
            { 'A', "Yes" },
            { 'B', "No" }
        };

        var question = Question.Create("Is this the real life?", false, questionVariants, user, gameResult).Value;

        var userGuess = QuestionUserGuess.Create(user, user2, question, [question.AnswerVariants.First(v => v.Notation == 'A')]);

        var result = question.RecordGuess(userGuess.Value);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void RecordGuess_GivenDuplicateUserGuess_ReturnsError()
    {
        var user = User.Create("player1").Value;
        var user2 = User.Create("player2").Value;
        var gameResult = Game.Create("test game", user).Value;

        var questionVariants = new Dictionary<char, string>()
        {
            { 'A', "Yes" },
            { 'B', "No" }
        };

        var question = Question.Create("Is this the real life?", false, questionVariants, user, gameResult).Value;

        var userGuess = QuestionUserGuess.Create(user, user2, question, [question.AnswerVariants.First(v => v.Notation == 'A')]);

        question.RecordGuess(userGuess.Value);

        var result = question.RecordGuess(userGuess.Value);
        result.IsSuccess.Should().BeFalse();
        result.Errors!.First().Message.Should().Be("User already made guess for this user");
    }

    [Fact]
    public void RecordGuess_GivenAllPeopleMadeChoiceAndAnswered_MarksQuestionAsAnswered()
    {
        var user = User.Create("player1").Value;
        var user2 = User.Create("player2").Value;
        var gameResult = Game.Create("test game", user).Value;

        gameResult.AddPlayer(user2);

        var questionVariants = new Dictionary<char, string>()
        {
            { 'A', "Yes" },
            { 'B', "No" }
        };

        var question = Question.Create("Is this the real life?", false, questionVariants, user, gameResult).Value;

        question.RecordChoice(QuestionUserChoice.Create(user, question, [question.AnswerVariants.First(v => v.Notation == 'A')]).Value);
        question.Answered.Should().BeFalse();
        question.RecordChoice(QuestionUserChoice.Create(user2, question, [question.AnswerVariants.First(v => v.Notation == 'A')]).Value);
        question.Answered.Should().BeFalse();


        question.RecordGuess(QuestionUserGuess.Create(user, user2, question, [question.AnswerVariants.First(v => v.Notation == 'A')]).Value);
        question.Answered.Should().BeFalse();
        question.RecordGuess(QuestionUserGuess.Create(user2, user, question, [question.AnswerVariants.First(v => v.Notation == 'A')]).Value);
        question.Answered.Should().BeTrue();
    }
}
