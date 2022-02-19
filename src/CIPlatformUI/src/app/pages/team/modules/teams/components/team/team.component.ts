import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TeamDto } from '../../../../../../services/team/team.interface';
import { Router } from '@angular/router';

@Component({
  selector: 'teams-team',
  templateUrl: './team.component.html',
  styleUrls: ['./team.component.scss'],
})
export class TeamComponent implements OnInit {
  @Input() team?: TeamDto;

  @Output() onJoin: EventEmitter<number> = new EventEmitter<number>();

  constructor(private readonly router: Router) {}

  ngOnInit(): void {}

  open(): void {
    this.router.navigate([`teams/${this.team?.id}`]);
  }

  join(): void {
    this.onJoin.emit(this.team?.id ?? -1);
  }
}
