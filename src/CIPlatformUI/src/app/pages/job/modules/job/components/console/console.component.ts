import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CreateTeamDto } from '../../../../../../services/team/team.interface';
import { SignalRService } from '../../../../../../services/signalr/signalr.service';

@Component({
  selector: 'job-console',
  templateUrl: './console.component.html',
  styleUrls: ['./console.component.scss'],
})
export class ConsoleComponent {
  @Input() log?: string[];

  @Output() onClose: EventEmitter<any> = new EventEmitter<any>();

  constructor() {}

  close(): void {
    this.onClose.emit();
  }
}
