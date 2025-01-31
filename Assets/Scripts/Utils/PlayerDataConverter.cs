using System.Linq;
using Data.Dto;
using Model;

namespace Utils
{
    public static class PlayerDataConverter
    {
        public static PlayerModel ToPlayerModel(PlayerDataDto playerDataDto)
        {
            return new PlayerModel(
                playerDataDto.expAmount,
                playerDataDto.complexityLevel,
                playerDataDto.cashAmount,
                playerDataDto.goldAmount,
                playerDataDto.currentCar,
                playerDataDto.boughtCars,
                ToAudioSettingsModel(playerDataDto.audioSettings),
                playerDataDto.previousStartUtcTimestamp,
                playerDataDto.currentStartUtcTimestamp,
                playerDataDto.sequentialDaysPlaying
                );
        }

        public static PlayerDataDto ToPlayerDataDto(PlayerModel playerModel)
        {
            return new PlayerDataDto(
                playerModel.ExpAmount,
                playerModel.ComplexityLevel,
                playerModel.CashAmount,
                playerModel.GoldAmount,
                (int)playerModel.CurrentCar,
                playerModel.BoughtCars.Select(c => (int)c).ToArray(),
                ToAudioSettingsDto(playerModel.AudioSettingsModel),
                playerModel.PreviousStartUtcTimestamp,
                playerModel.CurrentStartUtcTimestamp,
                playerModel.SequentialDaysPlaying);
        }

        private static AudioSettingsModel ToAudioSettingsModel(AudioSettingsDto dto)
        {
            return new AudioSettingsModel(
                dto.isSoundsMuted,
                dto.isMusicMuted,
                dto.soundsVolume,
                dto.musicVolume);
        }

        private static AudioSettingsDto ToAudioSettingsDto(AudioSettingsModel audioSettingsModel)
        {
            return new AudioSettingsDto(
                audioSettingsModel.IsSoundsMuted,
                audioSettingsModel.IsMusicMuted,
                audioSettingsModel.SoundsVolume,
                audioSettingsModel.MusicVolume);
        }
    }
}