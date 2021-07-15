# demux
Demux enables you to work with events in a data-oriented fashion.

# Why?
The popular way to handle events in Unity is to just use Action and use += for sub and -= for unsub.
It works, but it's difficult to keep track of subscriptions. Also there might be a problem when you want to unsubscribe lambda, if you didn't save it beforehand, it will remain subscribed forever.

## Example
Let's say you want to handle mouse events.
```csharp
public struct Key {
    public bool down;
    public bool up;
}

public struct MouseEvent {
    public Key? Left;
    public Key? Right;
}

// This class just holds demultiplexer
public class MouseDemux: MonoBehaviour {
    // demultiplexer receives event and calls all registered receivers
    public Demux<MouseEvent> demux = new Demux<MouseEvent>();
}

// This class pushes new events. As you can see it doesn't depend on our subscriber (Example class)
public class MouseProvider: MonoBehaviour {
    // gotta have this ref to push events
    public MouseDemux mouseDemux;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            // when you push an event, all registered handlers are called
            inputDemux.demux.Push(
                new MouseEvent {
                    Left = new Key {
                        down = true
                    }
                }
            );
        }

        if (Input.GetMouseButtonUp(0)) {
            inputDemux.demux.Push(
                new MouseEvent {
                    Left = new Key {
                        up = true
                    }
                }
            );
        }

        // provide other types of mouse input
        // ...
    }
}

// This class just prints messages if left mouse key was pressed or released
// As you can see it doesn't depend on MouseProvider, which generates events
public class Example: MonoBehaviour {
    // gotta have this ref to register handlers
    public MouseDemux mouseDemux;

    // you have to keep token if you are registered
    private Token printLeftToken;

    private void Awake() {
        // you must create your own token to subscribe
        // you can use the same token to subscribe multiple handlers if you want
        printLeftToken = new Token();
        // just call Register method on demultiplexer with token to register your handler
        // handler just accepts event, lambda would work here as well
        mouseDemux.Register(printLeftToken, PrintLeftDown);
    }

    // handler which just prints state of left mouse button
    public void PrintLeftDown(MouseEvent ev) {
        if (ev.Left.HasValue) {
            var leftKey = ev.Left.Value;
            if (leftKey.down) {
                Debug.Log("Left key is down");
            } else if (leftKey.up) {
                Debug.Log("Left key is up");
            }
        }
    }

    // don't forget to unsubscribe if MonoBehaviour got whackd
    private void OnDestroy() {
        if (printLeftToken != null) {
            printLeftToken.Cancel();
        }
    }
}
```
