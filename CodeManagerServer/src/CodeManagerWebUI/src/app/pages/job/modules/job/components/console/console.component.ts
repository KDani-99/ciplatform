import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CreateTeamDto } from '../../../../../../services/team/team.interface';
import { SignalRService } from '../../../../../../services/signalr/signalr.service';

@Component({
  selector: 'job-console',
  templateUrl: './console.component.html',
  styleUrls: ['./console.component.scss'],
})
export class ConsoleComponent implements OnInit {
  @Input() runId!: number;
  @Input() jobId!: number;
  @Input() step!: number;

  @Output() onClose: EventEmitter<any> = new EventEmitter<any>();

  constructor(private readonly signalrService: SignalRService) {}

  ngOnInit(): void {
    // TODO: send request to retrieve logs
    this.signalrService.connect().then(() => {
      this.signalrService.subscribeToChannel(1, 1);
    });
  }

  close(): void {
    this.onClose.emit();
  }
}
