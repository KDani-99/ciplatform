import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { CreateTeamDto, TeamDto } from '../../../../team.interface';

@Component({
  selector: 'teams-create-team',
  templateUrl: './create-team.component.html',
  styleUrls: ['./create-team.component.scss'],
})
export class CreateTeamComponent implements OnInit {
  @Output() onClose: EventEmitter<any> = new EventEmitter<any>();
  @Output() onCreate: EventEmitter<CreateTeamDto> =
    new EventEmitter<CreateTeamDto>();
  constructor() {}

  ngOnInit(): void {}

  close(): void {
    this.onClose.emit();
  }

  create(
    name: string,
    description: string,
    image: string,
    isPublic: boolean,
  ): void {
    this.onCreate.emit({
      name,
      description,
      image,
      isPublic,
    });
  }
}
