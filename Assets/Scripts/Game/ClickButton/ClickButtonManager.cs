using UnityEngine;

namespace Game.ClickButton
{
    public class ClickButtonManager : MonoBehaviour
    {
        [SerializeField] private ClickButton _clickButton;
        [SerializeField] private CriticalDamage _clickButtonCritical;
        [SerializeField] private Posion _posion;
        [SerializeField] private ClickButtonConfig _buttonConfig;
        [SerializeField] private PosionConfig _posionConfig;

        private float _chanceCrit;
        private bool _criticalCombo;
        private int _punhCrit;
        private int _critMissClicked = 3;

        private float _chancePosion;
        private bool _posionCombo;
        private int _punhPosion;
        private int _posionMissClicked = 4;

        private float _attack;
        private OilDamageType _oilDamageType;
        public void Initialize()
        {
            _posion.Initialize(_posionConfig);
            _posion.SubscribeOnClick(ShowPosion);
        }
        public void InitializeClickButton(float attack)
        {
            _attack = attack;
            _clickButton.Initialize(_buttonConfig.DefaultSprite, _buttonConfig.ButtonColors, _attack);
            _clickButton.SubscribeOnClick(ChanceCritical);
        }
        public void InitializeClickButtonCritical(float criticalAttack)
        {
            _clickButtonCritical.Initialize(_buttonConfig.CriticalSprite, criticalAttack + _attack * 1.5f,
                _buttonConfig.CriticalComboSprite, _buttonConfig.Range);
            _clickButtonCritical.SubscribeOnClick(NextCritical);
        }
        public void TakeOil(OilDamageType oilDamageType)
        {
            _oilDamageType = oilDamageType;
            _clickButton.TakeOil(oilDamageType);
            _clickButtonCritical.TakeOil(oilDamageType);
        }
        public void InitializeChanceCrit(float chanceCrit) => _chanceCrit = chanceCrit;
        public void InitializeChancePosion(float chancePosion) => _chancePosion = chancePosion;
        public void InitializeHealthPosion(int chancePosion) => _posion.TakeHealthPosion(chancePosion);
        public void InitializeManaPosion(int chancePosion) => _posion.TakeManaPosion(chancePosion);
        public void ChangePowerPosion(float change) => _posion.ChangePowerPosion(change);
        public void ChangeYPositionBdish(float change) =>
            _clickButtonCritical.ChangeYPositionBdish(change);
        public void AccumulatingCritDamage(float power) => _clickButtonCritical.AccumulatingCritDamage(power);
        public OilDamageType GetOil() => _oilDamageType;

        private void ChanceCritical()
        {
            if (_criticalCombo == false)
            {
                float chance = Random.Range(0f, 1f);
                if (chance < _chanceCrit)
                {
                    _clickButtonCritical.OnButton();
                    _criticalCombo = true;
                    _punhCrit = 0;
                }
            }
            else
            {
                if (_punhCrit > _critMissClicked)
                {
                    _clickButtonCritical.OffButton();
                    _criticalCombo = false;
                }
                _punhCrit++;
            }

            if (_posionCombo == false)
            {
                float chance = Random.Range(0f, 1f);
                if (chance < _chancePosion)
                {
                    _posion.OnButton();
                    _posionCombo = true;
                    _punhPosion = 0;
                }
            }
            else
            {
                if (_punhPosion > _posionMissClicked)
                {
                    _posion.OffButton();
                    _posionCombo = false;
                }
                _punhPosion++;
            }
        }
        private void NextCritical()
        {
            _clickButtonCritical.OnButton();
            if (_punhPosion < _posionMissClicked) _punhPosion++;
        }
        private void ShowPosion()
        {
            _posion.OnButton();
            if (_punhCrit < _critMissClicked) _punhCrit++;
        }

        public void ChangeAttack(float current) => _clickButton.ChangeAttack(current);
        public void ChangeAttack(float current, bool k) => _clickButton.ChangeAttack(current, k);
        public void ChangeCritAttack(float current) => _clickButtonCritical.ChangeAttack(current);
        public void ChangeChanceCrit(float chanceCrit)
        {
            _chanceCrit *= chanceCrit;
            if (_chanceCrit > 1) _chanceCrit = 1;
        }
        public void ChangeChancePosion(float chancePosion)
        {
            _chancePosion *= chancePosion;
            if (_chancePosion > 1) _chancePosion = 1;
        }

        public void ShowCrit() => _clickButtonCritical.OnButton();
        public void PublicShowPosion() => _posion.OnButton();
    }
}
