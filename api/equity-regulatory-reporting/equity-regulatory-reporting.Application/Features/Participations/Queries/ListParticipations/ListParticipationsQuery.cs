using equity_regulatory_reporting.Application.Common.Models;
using equity_regulatory_reporting.Application.Features.Participations.Dtos;
using MediatR;

namespace equity_regulatory_reporting.Application.Features.Participations.Queries.ListParticipations;

public record ListParticipationsQuery(PageRequest Page, Guid? CompanyId = null) : IRequest<PagedResult<ParticipationDto>>;
