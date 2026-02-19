using DijitalEvrakTakip.Application.Features.DeparmentFeatures.Commands.CreateDepartment;
using DijitalEvrakTakip.Domain.Dtos;
using DijitalEvrakTakip.Presentation.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;

namespace DijitalEvrakTakip.UnitTest
{
    public class DepartmentControllerUnitTest
    {
        [Fact]
        public async Task Create_ReturnsOkResult_WhenRequestIsValid()
        {
            //Arrange, tanımlamalar 
            var mediatorMock = new Mock<IMediator>();
            CreateDepartmentCommand createDepartmentCommand = new("unit test mock ", "");
            MessageResponse response = new("Birim başarıyla kaydedildi ...");
            CancellationToken cancellationToken = new();

            mediatorMock.Setup(m => m.Send(createDepartmentCommand, cancellationToken)).ReturnsAsync(response);

            DepartmentsController departmentController = new(mediatorMock.Object);

            //Act

            var result = await departmentController.Create(createDepartmentCommand, cancellationToken);

            //Assert, kontrol yapılacak alan
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<MessageResponse>(okResult.Value);

            Assert.Equal(response, returnValue);
            mediatorMock.Verify(m => m.Send(createDepartmentCommand, cancellationToken), Times.Once);
        }
    }
}
