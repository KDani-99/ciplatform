import { State } from '../../services/run/run.interface';
import { RunService } from '../../services/run/run.service';

interface Entity {
  state: State;
  startedDateTime: string;
  finishedDateTime: string;
}
interface EventEntity {
  state: State;
  dateTime: string;
}

export abstract class BaseRunComponent {
  protected constructor(public readonly runService: RunService) {}

  onReceiveResultEvent<T extends Entity, TEvent extends EventEntity>(
    obj: T | undefined,
    event: TEvent,
  ) {
    if (!obj) {
      return;
    }

    obj.state = event.state;

    if (event.state === State.RUNNING) {
      obj.startedDateTime = event.dateTime;
    } else {
      obj.finishedDateTime = this.runService.getTime(event.dateTime);
    }
  }
}
