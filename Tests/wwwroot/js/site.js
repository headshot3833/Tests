var answerCount = 0;

$(document).ready(function () {
    $('#addAnswerButton').click(function () {
        addAnswerField();
    });

    $('#answerContainer').on('click', '.btn-danger', function () {
        var answerIndex = $(this).closest('.form-group').index();
        removeAnswer(answerIndex);
    });
});

function removeAnswer(answerIndex) {
    $('[name="Answers[' + answerIndex + '].Text"]').closest('.form-group').remove();
    updateAnswerIndexes();
}

function addAnswerField() {
    var answerField = `
    <div class="form-group">
      <label>Answer ${answerCount + 1}:</label>
      <input type="text" name="Answers[${answerCount}].Text" class="form-control" required />
      <input type="radio" name="CorrectAnswerId" value="${answerCount}" required /> Correct
      <button type="button" class="btn btn-danger">Remove</button>
    </div>
  `;

    $('#answerContainer').append(answerField);
    updateAnswerIndexes();
    answerCount++;
}

function updateAnswerIndexes() {
    $('#answerContainer .form-group').each(function (index) {
        $(this).find('label').text('Answer ' + (index + 1) + ':');
        $(this).find('input[name^="Answers"]').attr('name', 'Answers[' + index + '].Text');
        $(this).find('input[type="radio"]').attr('name', 'CorrectAnswerId').val(index);
    });
}